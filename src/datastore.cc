#include "commons.h"
#include <fstream>

static void TransformData(std::vector<char> &data)
{
    static const std::string key = "RM7eTcr-nIpUPrB0a_V7G";

    for (size_t i = 0; i < data.size(); i++)
    {
        (uint8_t &)data[i] ^= (uint8_t)key[i % key.length()];
    }
}

static void LoadData(std::string &json)
{
    std::ifstream stream(utils::dataStorePath(), std::ios::binary);

    if (stream.good())
    {
        stream.seekg(0, std::ios::end);
        size_t fileSize = (size_t)stream.tellg();
        stream.seekg(0, std::ios::beg);

        std::vector<char> buffer(fileSize);
        stream.read(buffer.data(), fileSize);

        TransformData(buffer);
        json.assign(buffer.begin(), buffer.end());
    }
    else
    {
        json.assign("{}");
    }

    stream.close();
}

static void SaveData(std::string &json)
{
    std::ofstream stream(utils::dataStorePath(), std::ios::binary);

    if (stream.good())
    {
        std::vector<char> buffer(json.begin(), json.end());
        TransformData(buffer);
        stream.write(buffer.data(), buffer.size());
    }

    stream.close();
}

bool HandleDataStore(const std::wstring &fn,
    const std::vector<cef_v8value_t *> &args, cef_v8value_t * &retval)
{
    if (fn == L"LoadDataStore")
    {
        std::string json{};
        LoadData(json);

        cef_string_t result{};
        CefString_FromUtf8(json.c_str(), json.length(), &result);
        retval = CefV8Value_CreateString(&result);

        CefString_Clear(&result);
        return true;
    }
    else if (fn == L"SaveDataStore")
    {
        if (args.size() > 0 && args[0]->is_string(args[0]))
        {
            auto json = args[0]->get_string_value(args[0]);
            if (json && json->length > 0)
            {
                cef_string_utf8_t output{};
                CefString_ToUtf8(json->str, json->length, &output);
                std::string data(output.str, output.length);

                SaveData(data);
                CefString_UserFree_Free(json);
                CefString_ClearUtf8(&output);
            }
        }
        return true;
    }

    return false;
}