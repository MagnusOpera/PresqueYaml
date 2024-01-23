config ?= Debug
version ?= 0.0.0


build:
	dotnet build -c $(config)

dist:
	dotnet pack -c $(config) /p:Version=$(version) -o .out

test: dist
	dotnet test -c $(config) --logger "trx;LogFileName=test-results.trx"

publish: .out/*.nupkg
	@for file in $^ ; do \
		dotnet nuget push $$file -k $(nugetkey) -s https://api.nuget.org/v3/index.json --skip-duplicate ; \
    done
