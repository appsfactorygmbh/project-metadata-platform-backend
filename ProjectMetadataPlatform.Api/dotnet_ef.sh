#!/bin/sh

# How to use:
# sh ./dotnet_ef.sh <my_parameters>
# Example:
# sh ./dotnet_ef.sh migrations list
PMP_DB_URL=localhost PMP_DB_PORT=5432 PMP_DB_NAME=pmp_db_local PMP_DB_USER=postgres PMP_DB_PASSWORD=postgres dotnet ef -p ../ProjectMetadataPlatform.Infrastructure/ -s . $*
