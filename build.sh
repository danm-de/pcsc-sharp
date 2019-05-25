#!/bin/bash

if [ -z "$1" ]; then
    fake run build.fsx
else
    fake run build.fsx --target $1
fi