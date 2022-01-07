FROM dotnet5base
WORKDIR /home
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000;https://0.0.0.0:5001"]