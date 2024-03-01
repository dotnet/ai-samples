function set_dotnet_user_secrets() {
    local path="$1"
    local lines="$2"

    pushd "$path" >/dev/null
    dotnet user-secrets init
    dotnet user-secrets clear

    IFS=$'\n'
    for line in $lines; do
        name="${line%%=*}"
        value="${line#*=}"
        value="${value%\"}"
        value="${value#\"}"
        name="${name//__/:}"

        if [[ -n "$value" ]]; then
            dotnet user-secrets set "$name" "$value" >/dev/null
        fi
    done

    popd >/dev/null
}

lines=$(azd env get-values)
set_dotnet_user_secrets "./01-HikeSummary/" "$lines"
set_dotnet_user_secrets "./02-HikerAI/" "$lines"
set_dotnet_user_secrets "./03-ChattingAboutMyHikes/" "$lines"
# set_dotnet_user_secrets "./04-HikeTracker/" "$lines"
set_dotnet_user_secrets "./05-HikeImages/" "$lines"




