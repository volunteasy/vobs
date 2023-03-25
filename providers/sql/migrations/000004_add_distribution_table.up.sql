create table distribution (
    id bigint not null,
    org_id bigint not null,
    name varchar(50) not null,
    description text not null,
    date datetime not null,
    items int not null,
    benefits_allowed int not null,
    phone varchar(15) not null,
    address json not null,

    primary key (id),
    foreign key (org_id) references organizations(id)
);