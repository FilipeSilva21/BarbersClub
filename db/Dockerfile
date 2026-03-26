# Usamos a imagem oficial como base
FROM mcr.microsoft.com/mssql/server:2022-latest

# Mudar para o usuário root para ter permissão de instalar pacotes
USER root

# O comando de instalação corrigido
RUN apt-get update && \
    apt-get install -y curl apt-transport-https gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

# Voltar para o usuário padrão do mssql por segurança
USER mssql