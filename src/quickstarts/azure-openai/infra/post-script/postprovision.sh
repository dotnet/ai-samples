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

echo "hi"

lines=$(azd env get-values)
set_dotnet_user_secrets "./azure-openai/01-HikeBenefitsSummary/" "$lines"

set_dotnet_user_secrets "./semantic-kernel/01-HikeBenefitsSummary/" "$lines"
set_dotnet_user_secrets "./semantic-kernel/02-HikerAI/" "$lines"
set_dotnet_user_secrets "./semantic-kernel/03-ChattingAboutMyHikes/" "$lines"
set_dotnet_user_secrets "./semantic-kernel/04-HikerAIPro/" "$lines"
set_dotnet_user_secrets "./semantic-kernel/05-HikeImages/" "$lines"




