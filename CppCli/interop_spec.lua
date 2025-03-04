-- Specifies the generated interop code.

local M = {}

M.config =
{
    lua_lib_name = "luainterop",    -- for require
    host_lib_name = "HostInterop",  -- host filenames
    host_namespace = "Interop"      -- host namespace
}

------------------------ Host => Script ------------------------
M.script_funcs =
{
    {
        lua_func_name = "setup",
        host_func_name = "Setup",
        required = "true",
        description = "Initialize.",
        args =
        {
            { name = "opt", type = "Q", description = "Option" },
        },
        ret = { type = "I", description = "Return integer" }
    },

    {
        lua_func_name = "do_command",
        host_func_name = "DoCommand",
        required = "true",
        description = "Host executes arbitrary lua function.",
        args =
        {
            { name = "cmd", type = "S", description = "Specific command" },
            { name = "arg", type = "S", description = "Optional argument" },
        },
        ret = { type = "S", description = "Script response" }
    },
}

------------------------ Script => Host ------------------------
M.host_funcs =
{
    {
        lua_func_name = "log",
        host_func_name = "Log",
        description = "Script wants to log something.",
        args =
        {
            { name = "level", type = "I", description = "Log level" },
            { name = "msg", type = "S", description = "Log message" },
        },
        ret = { type = "I", description = "Unused" }
    },

    {
        lua_func_name = "notif",
        host_func_name = "Notification",
        description = "Script wants to say something.",
        args =
        {
            { name = "num", type = "I", description = "A number" },
            { name = "text", type = "S", description = "Some text" },
        },
        ret = { type = "I", description = "Unused" }
    },
}

return M
