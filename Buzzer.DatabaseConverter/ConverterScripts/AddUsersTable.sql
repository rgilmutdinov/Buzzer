create table Users
(
	ID integer primary key autoincrement not null,
	Login nvarchar(100) not null,
	Password nvarchar(100) not null
);

insert into Users (Login, Password) values ('Atai', 'WZRHGrsBESr8wYFZ9sx0tPURuZgG2lmzyvWpwXPKz8U=');