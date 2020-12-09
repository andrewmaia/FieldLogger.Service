CREATE TABLE Channel (
    ChannelId Bigint NOT NULL AUTO_INCREMENT,
    Tag varchar(128) NOT NULL,
    Value double NOT NULL,    
    Unit varchar(16) NOT NULL,
    Logged varchar(8) NOT NULL,    
    Date DateTime NOT NULL,
    PRIMARY KEY (ChannelId)
);