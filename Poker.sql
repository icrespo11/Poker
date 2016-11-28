create database poker;


drop table users;

create table users (
username varchar(200) Not Null,
password varchar(200) Not Null,
current_money integer Not Null,
highest_money integer Not Null,
privilege varchar(200) Not Null,
is_online bit Not Null Default 0,
constraint pk_users_username Primary Key(username)
);


insert into users values ('BrianCobb', 'password', 1000, 1000, 'admin', 0);
insert into users values ('Dan', 'password', 1000, 1000, 'admin', 0);
insert into users values ('Isaac', 'password', 1000, 1000, 'admin', 0);
insert into users values ('Roberto', 'password', 1000, 1000, 'admin', 0); 