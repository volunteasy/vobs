begin;
    create table if not exists organizations
    (
        id       bigint      not null,
        name     varchar(50) not null,
        document varchar(14) not null,
        phone    varchar(15) not null,
        address  json        not null,
        
        primary key (id),
        unique (document)
    );

    create table if not exists distributions
    (
        id               bigint      not null,
        org_id           bigint      not null,
        name             varchar(50) not null,
        description      text        not null,
        date             datetime    not null,
        items            int         not null,
        benefits_allowed int         not null,
        phone            varchar(15) not null,
        address          json        not null,
        
        primary key (id),
        foreign key (org_id) references organizations (id)
    );

    create table if not exists benefits
    (
        id                bigint      not null,
        assisted_id       varchar(36) not null,
        distribution_id   bigint      not null,
        queue_position_id bigint      not null,
        claimed_at        datetime    not null,
        
        primary key (id),
        unique (assisted_id, distribution_id),
        foreign key (distribution_id) references distributions (id)
    );

    create index idx_benefits_queue_position_id
        on benefits (queue_position_id);

    create table if not exists memberships
    (
        user_id varchar(36) not null,
        org_id  bigint      not null,
        role    varchar(10) not null,
        status  varchar(10) not null,
        
        primary key (user_id, org_id),
        foreign key (org_id) references organizations (id)
    );
commit;

