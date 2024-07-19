CREATE TABLE dbo.tb_user (
    id_user INT IDENTITY,
    dsc_nome VARCHAR(MAX) NOT NULL,
	lgn_usuario VARCHAR(50) NOT NULL,
    dat_nascimento DATETIME NOT NULL,
    dat_criacao DATETIME NOT NULL,
	psw_hash VARBINARY(MAX) NULL,
	psw_salt VARBINARY(MAX) NULL,
	id_tpperfil INT NOT NULL
    CONSTRAINT PK_TB_PACIENTE PRIMARY KEY (id_user)
);

CREATE TABLE dbo.tb_agendamento (
    id_agendamento INT IDENTITY,
    id_user INT NOT NULL,
    dat_agendamento DATE NOT NULL,
    hor_agendamento TIME NOT NULL,
    dsc_status INT NOT NULL,
    dat_criacao DATETIME NOT NULL,
    CONSTRAINT PK_TB_AGENDAMENTO PRIMARY KEY (id_agendamento)
);

ALTER TABLE dbo.tb_agendamento
ADD CONSTRAINT fk_agendamento_user FOREIGN KEY (id_user)
REFERENCES dbo.tb_user (id_user);

CREATE TABLE tb_tpperfil (
    id_tpperfil INT PRIMARY KEY,
    dsc_tpperfil VARCHAR(50) NULL
);


 