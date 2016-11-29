create database poker;


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


--insert into users values ('BrianCobb', 'password', 1000, 1000, 'admin', 0, 'dddddddd');
--insert into users values ('Dan', 'password', 1000, 1000, 'admin', 0, 'cccccccc');
--insert into users values ('Isaac', 'password', 1000, 1000, 'admin', 0, 'bbbbbbbb');
--insert into users values ('Roberto', 'password', 1000, 1000, 'admin', 0, 'aaaaaaaa'); 
--insert into users values ('ThatCrazyCow', 'password', 5000, 5000, 'admin', 0, 'arryarrg');
--insert into users values ('IWentBroke', 'password', 1, 1000, 'admin', 0, 'RickAstl');