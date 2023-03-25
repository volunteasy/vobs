create table memberships (
    user_id varchar(36) not null,
    org_id bigint not null,
    role varchar(10) not null,
    status varchar(10) not null,

    primary key (user_id, org_id),
    foreign key (org_id) references organizations (id)
);
