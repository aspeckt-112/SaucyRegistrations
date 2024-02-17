# Check to make sure dotnet is installed
if ! [ -x "$(command -v dotnet)" ]; then
  echo 'Error: dotnet is not installed.' >&2
  exit 1
fi

dotnet build src/SaucyRegistrations.Generators/SaucyRegistrations.Generators.csproj -c Release

dotnet pack src/SaucyRegistrations.Generators/SaucyRegistrations.Generators.csproj -c Release -o ./artifacts