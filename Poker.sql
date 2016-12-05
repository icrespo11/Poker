drop table hand_seat;
drop table hand_card_deck;
drop table hand_cards;
drop table hand_actions;
drop table table_players;
drop table hand;
drop table poker_table;
drop table users;


create table users (
username varchar(200) Not Null,
password varchar(200) Not Null,
current_money integer Not Null,
highest_money integer Not Null,
privilege varchar(200) Not Null,
is_online bit Not Null Default 0,
salt varchar(200) Not Null,

constraint pk_users_username Primary Key(username)
);


create table poker_table (
table_id integer identity Not Null,
host varchar(200) Not Null,
name varchar(200) Not Null,
min_bet integer Not Null,
max_bet integer Not Null,
ante integer Not Null,
max_buy_in integer Not Null,
pot integer Not Null,
dealer_position integer Not Null,
state_counter integer Not Null,
current_min_bet integer Not Null,

constraint pk_poker_table_table_id Primary Key(table_ID),
constraint fk_poker_table_users_host_username Foreign Key(host) References users(username),
);

create table table_players (
table_id integer Not Null,
player varchar(200) Not Null,
seat_number integer Not Null,
table_balance integer Not Null,
active bit Not Null,
occupied bit Not Null,

constraint pk_table_players_player_table_id Primary Key(table_ID, player),
constraint fk_table_players_poker_table_table_id Foreign Key(table_ID) References poker_table(table_ID),
constraint fk_table_players_users_players_username Foreign Key(player) References users(username),
);


create table hand (
hand_id integer identity Not Null,
table_id integer Not Null,

constraint pk_hand_hand_id Primary Key (hand_id),
constraint fk_hand_poker_table_table_id Foreign Key (table_id) References poker_table(table_id)
);


create table hand_cards (
hand_id integer Not Null,
player varchar(200) Not Null,
seat_number integer Not Null,
card_number integer Not Null,
card_suit varchar(8) Not Null,
dealt bit Not Null,
discarded bit Not Null,


constraint pk_hand_cards_hand_id_card_number_card_suit Primary Key (hand_id, card_number, card_suit),
constraint fk_hand_cards_users_player_username Foreign Key (player) References users (username),
constraint fk_hand_cards_player_hand_hand_id Foreign Key (hand_id) References hand (hand_id),
);


create table hand_actions (
hand_id integer Not Null,
player varchar(200) Not Null,
action varchar(50) Not Null,
round integer Not Null,

constraint pk_hand_actions_hand_id_player_round Primary Key (hand_id, player, round),
constraint fk_hand_actions_hand_hand_id Foreign Key (hand_id) References hand (hand_id),
constraint fk_hand_actions_users_player Foreign Key (player) References users (username),
);


create table hand_card_deck (
hand_id integer Not Null,
card_number integer Not Null,
card_suit varchar(8) Not Null,
dealt bit Not Null,
discarded bit Not Null,

constraint pk_hand_card_deck Primary Key (hand_id, card_number, card_suit),
constraint fk_hand_card_deck Foreign Key (hand_id) references hand (hand_id),
);

create table hand_seat(
table_id integer Not Null,
hand_id integer Not Null,
player varchar(200) Not Null,
current_bet integer Not Null,
is_turn bit Not Null,
discard_count integer Not Null,
has_discarded bit Not Null,
has_checked bit Not Null,
has_folded bit Not Null,

constraint pk_hand_seat Primary Key (table_id, hand_id, player),
constraint fk_hand_seat_hand_id Foreign Key (hand_id) references hand (hand_id),
constraint fk_hand_seat_player_table_id Foreign Key (table_id, player) references table_players (table_id, player),
);

insert into users values ('Brian', 'aWj+MjmYxgDJJs+nZ0I0UT/HXuo=', 1000, 1000, 'admin', 0, 'jX1cFHUweSs=');
insert into users values ('Dan', '1YHheZjlGm4OX6OwR6juPpk+DVA=', 1000, 1000, 'admin', 0, 'GWg3QKuxpCc=');
insert into users values ('Isaac', '+6ZmQLH549dDQLpRCe2gUzNJbLU=', 1000, 1000, 'admin', 0, 'PLxvjrzmYSY=');
insert into users values ('Roberto', '8UIpfDlbW6WP0t/0Uz2P4jp6EO8=', 1000, 1000, 'admin', 0, '9STlYHzStS8='); 
insert into users values ('ThatCrazyCow', 'LA0WSOFTrk+XK74D751oDcFe4fY=', 5000, 5000, 'admin', 0, 'd8xtGBE936c=');

insert into poker_table (host, name, min_bet, max_bet, ante, max_buy_in, pot, dealer_position, state_counter, current_min_bet) VALUES
('Dan', 'Bob the tester. Can we break it? Yes, we can!', 10, 20, 10, 1000, 0, 0, 0, 10),
('ThatCrazyCow', 'Moo, get out the way', 50, 1000, 50, 5000, 0, 0, 0, 50);

insert into table_players (table_ID, player, seat_number, table_balance, active, occupied) VALUES 
(1, 'Dan',  0, 500, 1, 1), (1, 'Isaac', 1, 400, 1, 1),
(2, 'ThatCrazyCow', 0, 2000, 1, 1), (2, 'Brian', 1, 1000, 1, 1);

insert into hand (table_id) values (1);
insert into hand (table_id) values (2);

insert into hand_cards values 
(1, 'Dan', 1, 2, 'hearts', 1, 0), (1, 'Dan', 1, 11, 'diamonds',1, 0), (1, 'Dan', 1, 6, 'hearts',1, 0), 
(1, 'Dan', 1, 8, 'clubs',1, 0), (1, 'Dan', 1, 1, 'diamonds',1, 0), (1, 'Isaac', 1, 9, 'hearts',1, 0),
(1, 'Isaac', 1, 2, 'spades',1, 0), (1, 'Isaac', 1, 10, 'diamonds', 1,0), (1, 'Isaac', 1, 13, 'diamonds',1, 0),
(1, 'Isaac', 1, 4, 'diamonds',1, 0);

insert into hand_seat VALUES(1, 1, 'dan', 20, 1, 0, 0, 0, 0);
insert into hand_seat VALUES(1, 1, 'isaac', 20, 0, 0, 0, 0, 0)


--SELECT * FROM users;
--SELECT * FROM poker_table;
--SELECT * FROM table_players;
--SELECT * from hand_actions;

