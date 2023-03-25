create table benefits (
    id bigint not null,
    assisted_id varchar(36) not null,
    distribution_id bigint not null,
    queue_position_id bigint not null,
    claimed_at datetime not null,

    primary key (id),
    foreign key (distribution_id) references distribution(id)
);