:- consult('cars_db.pl').
:- dynamic car/9.

% Задаем вопросы
ask_question(1, 'Тип кузова?', ['1. Купе', '2. Седан', '3. Хэтчбек']).
ask_question(2, 'Объем двигателя?', ['1. 1-2 литра', '2. 2-3 литра', '3. 3-4 литра', '4. 4-5 литра']).
ask_question(3, 'Тип двигателя?', ['1. Бензин', '2. Дизель', '3. Электрический']).
ask_question(4, 'Мощность?', ['1. 50-100 л.с.', '2. 100-200 л.с.', '3. 200-300 л.с.', '4. 300-400 л.с.', '5. 400-500 л.с.']).
ask_question(5, 'Страна производства?', ['1. Россия', '2. США', '3. Япония', '4. Германия']).
ask_question(6, 'Привод?', ['1. Передний', '2. Задний', '3. Полный']).
ask_question(7, 'Коробка передач?', ['1. Механика', '2. Автомат']).
ask_question(8, 'Быстрый?', ['1. Да', '2. Нет']).

% Ввод ответа пользователя
ask_user(Q, Answer) :-
    ask_question(Q, Question, Options),
    format('~w ~w~n', [Question, Options]),
    read(Answer).

% Проверка префикса
prefix([], _).
prefix([H|T], [H|T2]) :-
    prefix(T, T2).

% Поиск всех машин, подходящих под текущий префикс
matching_prefix(PartialAnswers, Matches) :-
    findall(Car, (
        car(Car, T, V, D, P, C, G, F, B),
        Full = [T, V, D, P, C, G, F, B],
        prefix(PartialAnswers, Full)
    ), Matches).

% Угадывание
guess_car(Answers) :-
    car(Car, T, V, D, P, C, G, F, B),
    Answers = [T, V, D, P, C, G, F, B],
    format('Угадал: ~w~n', [Car]).

% Добавление новой машины
add_car(Name, T, V, D, P, C, G, F, B) :-
    atom(Name),
    assertz(car(Name, T, V, D, P, C, G, F, B)),
    format('Автомобиль ~w добавлен в базу данных!~n', [Name]).

% Основной игровой цикл
start_game :-
    ask_loop([], FinalAnswers),
    (   guess_car(FinalAnswers)
    ->  true
    ;   format('Не удалось угадать! Хотите добавить новый автомобиль? (да/нет)~n'),
        read(Response),
        (   Response = 'да'
        ->  format('Введите марку автомобиля (введите в кавычках, например \'BMW 8 Series\'): '),
            read(Name),
            FinalAnswers = [T, V, D, P, C, G, F, B],
            add_car(Name, T, V, D, P, C, G, F, B)
        ;   format('Спасибо за игру!~n')
        )
    ).

ask_loop(AnswersSoFar, FinalAnswers) :-
    length(AnswersSoFar, N),
    matching_prefix(AnswersSoFar, Matches),
    (   Matches = [OnlyCar]
    ->  format('Вы загадали ~w? (да/нет)~n', [OnlyCar]),
        read(Resp),
        (   Resp = 'да'
        ->  FinalAnswers = AnswersSoFar
        ;   NextQ is N + 1,
            ask_user(NextQ, Ans),
            append(AnswersSoFar, [Ans], NewAnswers),
            ask_loop(NewAnswers, FinalAnswers)
        )
    ;   (N >= 8 ->
            FinalAnswers = AnswersSoFar
        ;   NextQ is N + 1,
            ask_user(NextQ, Ans),
            append(AnswersSoFar, [Ans], NewAnswers),
            ask_loop(NewAnswers, FinalAnswers)
        )
    ).

% API

ask(Answers) :-
    matching_prefix(Answers, Matches),
    ( Matches = [OnlyCar] ->
        format('guess:~w', [OnlyCar]), !
    ; true ),
    car(Car, T, V, D, P, C, G, F, B),
    Answers = [T, V, D, P, C, G, F, B],
    format('guess:~w', [Car]), !.

ask(Answers) :-
    length(Answers, N),
    N < 8,
    QIndex is N + 1,
    ask_question(QIndex, QText, Options),
    format('question:~w ~w', [QText, Options]), !.

ask(_) :-
        format('not_found'), !, halt.

add_object(Name, Answers) :-
    Answers = [T, V, D, P, C, G, F, B],
    assertz(car(Name, T, V, D, P, C, G, F, B)),
    save_cars,
    write('success').

save_cars :-
    tell('cars_db.pl'),
    listing(car/9),
    told.