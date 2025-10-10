-- Specifies the generated interop code.

local M = {}

M.config =
{
    lua_lib_name = "luainterop",    -- for require
    class_name = "Interop",         -- host filenames
}

------------------------ Host => Script ------------------------
M.script_funcs =
{
    {
        lua_func_name = "setup",
        host_func_name = "Setup",
        description = "Initialize stuff",
        args =
        {
            { name = "opt", type = "I", description = "Option" },
        },
        ret = { type = "I", description = "Return integer" }
    },

    {
        lua_func_name = "do_command",
        host_func_name = "DoCommand",
        description = "Arbitrary lua function with all arg types",
        args =
        {
            { name = "cmd",   type = "S", description = "Specific command" },
            { name = "arg_B", type = "B", description = "bool argument" },
            { name = "arg_I", type = "I", description = "int argument" },
            { name = "arg_N", type = "N", description = "number/double argument" },
            { name = "arg_S", type = "S", description = "string argument" },
        },
        ret = { type = "S", description = "Function response" }
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
            { name = "msg",   type = "S", description = "Log message" },
        },
        ret = { type = "I", description = "Unused" }
    },

    {
        lua_func_name = "notif",
        host_func_name = "Notification",
        description = "Script wants to say something.",
        args =
        {
            { name = "arg_I", type = "I", description = "A number" },
            { name = "arg_S", type = "S", description = "Some text" },
            { name = "arg_B", type = "B", description = "boooooool" },
            { name = "arg_N", type = "N", description = "numero/doublo" },
        },
        ret = { type = "I", description = "Back at you" }
    },
}

return M
