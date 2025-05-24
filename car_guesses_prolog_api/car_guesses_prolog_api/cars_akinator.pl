:- consult('cars_db.pl').
:- consult('cars_db_with_distinguish.pl').

:- dynamic car/9.
:- dynamic distinguish/5.
:- dynamic pending_distinction/3.

% Вопросы
ask_question(1, 'Тип кузова?', ['1. Купе', '2. Седан', '3. Хэтчбек']).
ask_question(2, 'Объем двигателя?', ['1. 1-2 литра', '2. 2-3 литра', '3. 3-4 литра', '4. 4-5 литра']).
ask_question(3, 'Тип двигателя?', ['1. Бензин', '2. Дизель', '3. Электрический']).
ask_question(4, 'Мощность?', ['1. 50-100 л.с.', '2. 100-200 л.с.', '3. 200-300 л.с.', '4. 300-400 л.с.', '5. 400-500 л.с.']).
ask_question(5, 'Страна производства?', ['1. Россия', '2. США', '3. Япония', '4. Германия']).
ask_question(6, 'Привод?', ['1. Передний', '2. Задний', '3. Полный']).
ask_question(7, 'Коробка передач?', ['1. Механика', '2. Автомат']).
ask_question(8, 'Быстрый?', ['1. Да', '2. Нет']).

% Ввод
ask_user(Q, Answer) :-
    ask_question(Q, Text, Opts),
    format('~w ~w~n', [Text, Opts]),
    read(Answer).

prefix([], _).
prefix([H|T], [H|T2]) :- prefix(T, T2).

matching_prefix(Answers, Matches) :-
    findall(Car, (
        car(Car, T, V, D, P, C, G, F, B),
        prefix(Answers, [T, V, D, P, C, G, F, B])
    ), Matches).

% Сначала обработка distinguish
ask(Answers) :-
    matching_prefix(Answers, Matches),
    length(Answers, 8),
    findall((X1, X2, QText, Opts, Correct), (
        distinguish(X1, X2, QText, Opts, Correct),
        member(X1, Matches),
        member(X2, Matches),
        X1 \= X2
    ), [(X1, X2, QText, Opts, Correct)|_]),
    format('distinguish:~w:~w:~w:~w:~w~n', [X1, X2, Correct, QText, Opts]),
    asserta(pending_distinction(X1, X2, Correct)),
    !.

ask([Ans|_]) :-
    retract(pending_distinction(X1, X2, Correct)),
    ( Ans =:= Correct -> format('guess:~w', [X2])
    ; format('guess:~w', [X1]) ), !.

% Стандартное угадывание
ask(Answers) :-
    matching_prefix(Answers, Matches),
    ( Matches = [OnlyCar] ->
        format('guess:~w', [OnlyCar]), !
    ; true ),
    car(Car, T, V, D, P, C, G, F, B),
    Answers = [T, V, D, P, C, G, F, B],
    format('guess:~w', [Car]), !.

% Следующий вопрос
ask(Answers) :-
    length(Answers, N),
    N < 8,
    Q is N + 1,
    ask_question(Q, Qtext, Opts),
    format('question:~w ~w', [Qtext, Opts]), !.

ask(_) :- format('not_found'), !, halt.


% Добавление машины и различающего вопроса
add_object_with_question(Name, BaseAnswers, QuestionText, Options, CorrectForNew) :-
    BaseAnswers = [T, V, D, P, C, G, F, B],

    % Найти все машины с такими же ответами
    findall(Car, (
        car(Car, T2, V2, D2, P2, C2, G2, F2, B2),
        [T2, V2, D2, P2, C2, G2, F2, B2] = BaseAnswers
    ), Matches),

    % Добавить новую машину, если её ещё нет
    ( member(Name, Matches) -> true
    ; assertz(car(Name, T, V, D, P, C, G, F, B)),
      open('cars_db.pl', append, S),
      format(S, 'car(~q,~w,~w,~w,~w,~w,~w,~w,~w).~n', [Name, T, V, D, P, C, G, F, B]),
      close(S)
    ),

    % Добавить различающие вопросы между новой машиной и всеми похожими
    forall((member(Other, Matches), Other \= Name), (
        Opposite is 3 - CorrectForNew,
        D1 = distinguish(Other, Name, QuestionText, Options, CorrectForNew),
        D2 = distinguish(Name, Other, QuestionText, Options, Opposite),
        assertz(D1), assertz(D2),
        append_distinguish(D1),
        append_distinguish(D2)
    )),

    format('added_with_question'), !.

append_distinguish(D) :-
    open('cars_db_with_distinguish.pl', append, Stream),
    portray_clause(Stream, D),
    close(Stream).


add_object(Name, Answers) :-
    Answers = [T, V, D, P, C, G, F, B],
    assertz(car(Name, T, V, D, P, C, G, F, B)),
    save_cars,
    write('success').