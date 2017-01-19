#!/bin/sh
# Deploy to staging

#   Need to import:
#       - a list of servers 
#       - name of the indexer server
#       - Version number to run
#       - SSH credentials
#
#   Steps
#
#    Suspend cron on Indexer server (For images having an indexer)
#       SSH to designated server, create STOP file(s)
#
#    Wait for any running tasks to complete (Look for the container with the indexer)
#
#    Per server
#
#        Deploy configuration (Write a persistent something or other telling the system which tag it's going to use)
#        Stop existing API container
#           Use tools/run/halt-container.sh
#
#        Pull image for new version (pull version-specific tag)
#            When we run the image, possibly run the indexer first.
#               tools/run/bestbets-indexer.sh bestbets.indexer.config.live (or .preview)
#            This is something we'd want when changing the schema, probably involves introducing a new alias at the same time (said new alias would have no data until the indexer runs)
#
#        Start API via tools/run/bestbets-api.sh bestbets.api.config.live (or .preview)
#
#        Test API availability
#            If all is well,
#                Remove old image
#                Continue on next server.
#            Error:
#                Roll back to previous image
#
#    After all servers are updated, report that deployment has completed.
#
#    Run indexer(?) (Command line switch?)
#
#    Resume cron
#       SSH to designated server, remove STOP file(s)
