create table organizations (
    id bigint not null,
    name varchar(50) not null,
    document varchar(14) not null,
    phone varchar(15) not null,
    address json not null,

    primary key(id),
    unique(document)
);