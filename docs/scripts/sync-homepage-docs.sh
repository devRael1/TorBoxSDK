#!/usr/bin/env bash

set -euo pipefail

owner="${1:-devRael1}"
repository="${2:-TorBoxSDK}"
branch="${3:-main}"

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
docs_root="$(cd -- "$script_dir/.." && pwd)"
repository_root="$(cd -- "$docs_root/.." && pwd)"
source_path="$script_dir/homepage-content.md"
readme_path="$repository_root/README.md"
docs_index_path="$docs_root/index.md"

shared_content="$(perl -0pe 's/\s+\z//' "$source_path")"

readme_content="$shared_content
"

docs_body="$(printf '%s' "$shared_content" | SYNC_OWNER="$owner" SYNC_REPOSITORY="$repository" SYNC_BRANCH="$branch" perl -0pe '
  my $owner = $ENV{"SYNC_OWNER"};
  my $repository = $ENV{"SYNC_REPOSITORY"};
  my $branch = $ENV{"SYNC_BRANCH"};
  s{\[(?<text>[^\]]+)\]\((?<target>[^)]+)\)}{
    my $text = $+{text};
    my $target = $+{target};

    if ($target =~ m{^(?:https?://|#)}) {
      "[$text]($target)";
    }
    elsif ($target =~ m{^docs/}) {
      my $relative = substr($target, 5);
      "[$text]($relative)";
    }
    else {
      my $path_kind = $target =~ m{/$} ? q{tree} : q{blob};
      "[$text](https://github.com/$owner/$repository/$path_kind/$branch/$target)";
    }
  }ge;
' )"

docs_index_content="---
uid: index
title: TorBoxSDK Documentation
description: Unofficial C# SDK for the TorBox API with typed clients for Main, Search, and Relay APIs.
---

$docs_body
"

printf '%s' "$readme_content" > "$readme_path"
printf '%s' "$docs_index_content" > "$docs_index_path"
