# SetiMarine - SETICOM Tecnologia

## Portas
- Blazor: 9092 | PostgreSQL: 5436

## Imagens necessarias
Coloque em SetiMarine.Web/wwwroot/images/

### boats/ - hero
- boat-main.jpg  (lancha, centro)
- boat-small.jpg (barco menor, esquerda)
- boat-sail.jpg  (veleiro, direita)

### boats/top/ - mapa visual (vista superior, fundo transparente)
- lancha-top.png
- veleiro-top.png

### marina/ - backgrounds sutis
- marina-aerial.jpg
- marina-sunset.jpg

### flags/ - seletor de idioma
- br.png (20x14px)
- us.png (20x14px)
- es.png (20x14px)

## Migrations
    cd SetiMarine.Web
    dotnet ef migrations add Inicial --project ../SetiMarine.Infrastructure
    dotnet ef database update

## Deploy
    git add . && git commit -m "feat: inicial" && git push
