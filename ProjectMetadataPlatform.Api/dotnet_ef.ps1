# How to use:
# pws ./dotnet_ef.ps1 <my parameters>
# Example:
# pws ./dotnet_ef.ps1 migrations list
pwsh -CommandWithArgs '$env:PMP_DB_URL="localhost";$env:PMP_DB_PORT = "5432";$env:PMP_DB_NAME = "pmp_db_local";$env:PMP_DB_USER = "postgres";$env:PMP_DB_PASSWORD = "postgres"; dotnet ef -p ..\ProjectMetadataPlatform.Infrastructure\ -s . $args' $args
