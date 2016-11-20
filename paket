#!/bin/bash

root=$(dirname "$0")
dir=$root/.paket
bootstrapper=$dir/paket.bootstrapper.exe
paket=$dir/paket.exe

maybe_mono=mono
if [ "$OS" == "Windows_NT" ]; then
  # Use .NET on Windows
  maybe_mono=
fi

if [ ! -f "$paket" ]; then
  $maybe_mono "$bootstrapper"
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    exit $exit_code
  fi
fi

$maybe_mono "$paket" $*
exit_code=$?
if [ $exit_code -ne 0 ]; then
  exit $exit_code
fi
