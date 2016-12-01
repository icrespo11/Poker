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

constraint pk_poker_table_table_id Primary Key(table_ID),
constraint fk_poker_table_users_host_username Foreign Key(host) References users(username),
);

create table table_players (
table_id integer Not Null,
player varchar(200) Not Null,

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

insert into users values ('BrianCobb', 'password', 1000, 1000, 'admin', 0, 'dddddddd');
insert into users values ('Dan', 'password', 1000, 1000, 'admin', 0, 'cccccccc');
insert into users values ('Isaac', 'password', 1000, 1000, 'admin', 0, 'bbbbbbbb');
insert into users values ('Roberto', 'password', 1000, 1000, 'admin', 0, 'aaaaaaaa'); 
insert into users values ('ThatCrazyCow', 'password', 5000, 5000, 'admin', 0, 'arryarrg');
insert into users values ('IWentBroke', 'password', 1, 1000, 'admin', 0, 'RickAstl');

insert into poker_table (host, name, min_bet, max_bet, ante) VALUES
('Dan', 'Bob the tester. Can we break it? Yes, we can!', 10, 20, 10),
('ThatCrazyCow', 'Moo, get out the way', 50, 1000, 50);

insert into table_players (table_ID, player) VALUES 
(1, 'Dan'), (1, 'Isaac'), (1, 'IWentBroke'),
(2, 'ThatCrazyCow'), (2, 'BrianCobb');

SELECT * FROM users;
SELECT * FROM poker_table;
SELECT * FROM table_players;