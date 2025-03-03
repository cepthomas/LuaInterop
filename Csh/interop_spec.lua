-- Spec to gen C# interop code.
-- All description and args fields are optional.
-- A return value is required, even if just a dummy.
-- Supported data types: B->bool  I->int  N->double  S->string T->TableEx


local M = {}

M.config =
{
    lua_lib_name = "luainterop",            -- for require
    host_lib_name = "Interop",              -- host filenames
    host_namespace = "Interop",             -- host namespace
    add_refs = { "System.Diagnostics", },   -- for using (optional)
}

-- Host calls script.
M.script_funcs =
{
    {
        lua_func_name = "my_lua_func",
        host_func_name = "MyLuaFunc",
        required = "true",
        description = "Tell me something good.",
        args =
        {
            {
                name = "arg_one",
                type = "S",
                description = "some strings"
            },
            {
                name = "arg_two",
                type = "I",
                description = "a nice integer"
            },
            {
                name = "arg_three",
                type = "T",
            },
        },
        ret =
        {
            type = "T",
            description = "a returned thing"
        }
    },

    {
        lua_func_name = "my_lua_func2",
        host_func_name = "MyLuaFunc2",
        required = "true",
        description = "wooga wooga",
        args =
        {
            {
                name = "arg_one",
                type = "B",
                description = "aaa bbb ccc"
            },
        },
         ret =
        {
            type = "N",
            description = "a returned number"
        }
    },

    {
        lua_func_name = "no_args_func",
        host_func_name = "NoArgsFunc",
        required = "true",
        description = "no_args",
        ret =
        {
            type = "N",
            description = "a returned number"
        },
    },

    {
        lua_func_name = "optional_func",
        host_func_name = "OptionalFunc",
        required = "false",
        description = "Function is optional.",
        ret =
        {
            type = "I",
            description = "Dummy return value."
        }
    },

}

-- Script calls host.
M.host_funcs =
{
    {
        lua_func_name = "get_time",
        host_func_name = "GetTime",
        description = "What time is it",
        ret =
        {
            type = "S",
            description = "The time"
        }
    },

    {
        lua_func_name = "check_value",
        host_func_name = "CheckValue",
        description = "Val1 is greater than val2? with no args",
        args =
        {
            {
                name = "val_one",
                type = "N",
                description = "Val 1"
            },
            {
                name = "val_two",
                type = "N",
                description = "Val 2"
            },
        },
        ret =
        {
            type = "B",
            description = "The answer"
        }
    },
}

return M
