Write-Host "🔍 Mapeando estrutura do projeto..." -ForegroundColor Cyan

# Lista todos os arquivos
Get-ChildItem -Recurse -File |
    Select-Object FullName |
    Out-File estrutura.txt

Write-Host "📁 Estrutura salva em estrutura.txt"

# Pega arquivos importantes (C#, Razor, config)
$extensoes = "*.cs","*.razor","*.json","*.csproj"

Write-Host "📄 Lendo arquivos principais..." -ForegroundColor Cyan

foreach ($ext in $extensoes) {
    Get-ChildItem -Recurse -Filter $ext | ForEach-Object {
        Add-Content projeto_detalhado.txt "`n===================="
        Add-Content projeto_detalhado.txt "Arquivo: $($_.FullName)"
        Add-Content projeto_detalhado.txt "===================="
        Get-Content $_.FullName | Add-Content projeto_detalhado.txt
    }
}

Write-Host "✅ Arquivo projeto_detalhado.txt gerado com sucesso!"