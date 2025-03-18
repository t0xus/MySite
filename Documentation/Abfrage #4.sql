CREATE TABLE resume_content(
id SERIAL PRIMARY KEY,
position_title VARCHAR(40),
place VARCHAR(30),
company VARCHAR(40),
date_from DATE,
date_till DATE,
id_rl smallint
)


SELECT * FROM resume_lifestage

SELECT * FROM resume_content





CREATE TABLE resume_contact(
id SERIAL PRIMARY KEY,
NAME VARCHAR(100),
Email VARCHAR(100),
Nachricht TEXT

)




CREATE TABLE resume_login(
id SERIAL PRIMARY KEY,
Username VARCHAR(100),
PWHash VARCHAR(100)

)


SELECT * FROM resume_content