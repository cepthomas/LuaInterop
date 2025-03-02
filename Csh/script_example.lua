
local li = require("luainterop")

print("Loading script_example.lua!")


----------------- Functions called from C#. -----------------
function my_lua_func(arg_one, arg_two, arg_three)
    --(string arg_one, int arg_two, TableEx arg_three)
    print("my_lua_func ")
    ret = {}
    return ret
end

function my_lua_func2(arg_one)
    --(bool arg_one)
    print("my_lua_func2 ")
    ret = {}
    return 99.99
end

function no_args_func()
    print("no_args_func ")
    return 888.88
end

function optional_func()
    print("optional_func ")
    return 12345
end


function do_cb()
    print("optional_func ")
    return 12345
end



--[[
        #region ============= C# => Lua functions =============
        public TableEx? MyLuaFunc(string arg_one, int arg_two, TableEx arg_three)
                     LuaType ltype = _l.GetGlobal("my_lua_func");

        public double? MyLuaFunc2(bool arg_one)
                     LuaType ltype = _l.GetGlobal("my_lua_func2");

        public double? NoArgsFunc()
                     LuaType ltype = _l.GetGlobal("no_args_func");

        public int? OptionalFunc()
            LuaType ltype = _l.GetGlobal("optional_func");


        #region ============= Lua => C# callback functions =============s
            _MyLuaFunc3 = _instance!.MyLuaFunc3;
            _libFuncs.Add(new LuaRegister("my_lua_func3", _MyLuaFunc3));
        args =
        {
            {
                name = "arg_one",
                type = "N",
                description = "kakakakaka"
            },
        },
        ret =
        {
            type = "B",
            description = "required return value"
        }




            _FuncWithNoArgs = _instance!.FuncWithNoArgs;
            _libFuncs.Add(new LuaRegister("func_with_no_args", _FuncWithNoArgs));
        ret =
        {
            type = "N",
            description = "a returned thing"
        }

]]




--[[
local M = {}

-- Functions implemented in C#.
print("Loading luaex_mod.lua!")

-- Local vars.
local yikes = "xxxxxxx"

-- Global vars.
g_int = 71717


-- Module vars.
M.m_string = "Here I am"
M.m_bool = false
M.m_table = { dev_type="bing_bong", channel=10, abool=true }
M.m_list_int = { 2, 56, 98, 2 }


-- Functions called from C#.
function M.funcmod(s)
    print("funcmod " .. #s)
    return #s + 3
end

function M.calcmod(addends, suffix)
    sum = 0
    for k, v in pairs(addends) do
        print(k .. ":" .. v)
        sum = sum + v
    end
    return { str = string.format('>>>%d_%s<<<', sum, suffix), sum = sum }
end


----------------------------------------------------------------------------
-- Module initialization.

-- Seed the randomizer.
local seed = os.time()
math.randomseed(seed)
M.seed = seed

-- Return the module.
return M
]]


--[[
----------------------------------------------------------------------------
-- Other
----------------------------------------------------------------------------
local api = require("api_lib") -- C# module

-- Functions called by lua implemented in C#.
v = api.printex("Loading luaex.lua!")
y = api.timer(true)

-- Functions called from C#.
function do_operation(arg_one, arg_two)
    api.printex("do_operation(): " .. arg_one .. " " .. arg_two)
    ret = { sret = arg_one:reverse(), iret = arg_two / 2 }
    return ret
end

-- How long is it?
local msec = api.timer(false)
api.printex("this took " .. msec .. " msec")
]]



--[[
----------------------------------------------------------------------------
-- script+main.lua  C version
----------------------------------------------------------------------------

-- Test script.

local gen = require("luainterop") -- lua-C api

local script_cnt = 0

local days = { "Hamday", "Eggday", "Moonday", "Boogaday" }

-- print("=============== go go go =======================")


--------------------- Lua calls C host -----------------------------------

local ts = gen.get_timestamp()

local senv = gen.get_environment(27.34)

local b = gen.log(1, string.format("I know this: ts:%d env:%s", ts, senv))

--------------------- C host calls Lua -----------------------------------

-----------------------------------------------------------------------------
function calculator(op_one, oper, op_two)
    if oper == "+" then
        return op_one + op_two
    elseif oper == "-" then
        return op_one - op_two
    elseif oper == "*" then
        return op_one * op_two
    elseif oper == "/" then
        return op_one / op_two
    else
        error("Invalid operator "..oper)
    end
end

-----------------------------------------------------------------------------
function day_of_week(day)
    for i, v in ipairs(days) do
        if v == day then
            return i
        end
    end
    return 0
end

-----------------------------------------------------------------------------
function first_day()
    return days[1]
end

-----------------------------------------------------------------------------
function invalid_func_not()
    return 1.23
end

-----------------------------------------------------------------------------
function invalid_arg_type(arg1)
    -- Spec says arg1 is a string, script thinks it is an int.
    print('scr:', arg1)
    print('scr:', arg1 + 5)
    return arg1 + 5
end

-----------------------------------------------------------------------------
function invalid_ret_type()
    -- Spec says ret is an int, script thinks it is a string.
    return 'xyz'
end

-----------------------------------------------------------------------------
function error_func(flavor)
    if flavor == 1 then
        return user_lua_func1()
    else
        return gen.force_error()
    end
end

----------------------- Internal user lua functions -------------------------

-----------------------------------------------------------------------------
function user_lua_func3()
    error("user_lua_func3() raises error()")
    script_cnt = script_cnt + 1
    return script_cnt
end

-----------------------------------------------------------------------------
function user_lua_func2()
    return user_lua_func3()
end

-----------------------------------------------------------------------------
function user_lua_func1()
    return user_lua_func2()
end

]]


