config ?= Debug
version ?= 0.0.0

DOTNET=./.dotnet/dotnet


build:
	$(DOTNET) build -c $(config)

dist:
	$(DOTNET) pack -c $(config) /p:Version=$(version) -o .out

test: dist
	$(DOTNET) test -c $(config) --logger "trx;LogFileName=test-results.trx"

publish: .out/*.nupkg
	@for file in $^ ; do \
		$(DOTNET) nuget push $$file -k $(nugetkey) -s https://api.nuget.org/v3/index.json --skip-duplicate ; \
    done

install:
	./scripts/dotnet-install.sh --channel 9.0.1xx --quality preview --install-dir .dotnet/
