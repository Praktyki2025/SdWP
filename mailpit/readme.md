### Post scriptum
Ze względu na to że nie mogę używać Gmail do testowania maili postawnowiłem zrobić prosty kontener docker do testowania maili 
~Mati

## Jak uruchomić?

1. Pobierz docker z oficjalnej strony docker https://www.docker.com/
2. Zmień nazwę z `.env.example` na `.env`
3. Otwórz konsole w tym folderze i wykonaj następujące komendy
```shell
docker-compose build 
docker-compose run
```
Mailpit będzie dostępny pod adresem `http://localhost:63854/`

W Program.cs umieściłem prosty Test do sprawdzania czy maile działają. Jak uruchomicie Mailpit a po tym aplikacje to będzie go widać. 