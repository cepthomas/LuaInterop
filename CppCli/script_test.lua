-- Use the debugger. The project must be set to `<OutputType>Exe</OutputType>` even if using WinForms.
local dbg = require("debugex")
-- Set any config here.
-- dbg.pretty_depth = 4
-- dbg.auto_where = 4
-- dbg.ansi_color = true
-- dbg.trace = false
-- dbg.init(59120)
dbg.init()

local li  = require("luainterop")

local counter = 100

---@diagnostic disable: lowercase-global, unused-local, unused-function

-----------------------------------------------------------------------------
--- Log functions. This goes straight through to the host.
-- Magic numbers must match host code.
-- @param msg what to log
local function log_error(msg) li.log(4, msg) end
local function log_warn(msg)  li.log(3, msg) end
local function log_info(msg)  li.log(2, msg) end
local function log_debug(msg) li.log(1, msg) end
local function log_trace(msg) li.log(0, msg) end

log_info('Loading script_test.lua')
--> SCR_LOG Loading script_test.lua 

-- Test event.
li.notif(33, "Notification from script_test")
--> SCR_NOT Notification from script_test(33) 

--uncomment broken here
--> LUA_EXC ScriptSyntaxError: Load script file failed. [C:\Dev\Libs\LuaInterop\CppCli\script_test.lua:32: syntax error near 'here'] 

-----------------------------------------------------------------------------
function setup()
    log_info('setup() was called')
    --> SCR_LOG setup() was called 
    return 3
end

-----------------------------------------------------------------------------
local function boom()
    log_info('boom() was called')
    local ret = 'boom'..nil
    return ret
end


-----------------------------------------------------------------------------
function do_command(cmd, arg)
    local ret = '???'

    if cmd == 'do_math' then
        log_info('Got do_math: '..cmd..'('..arg..')')
        ret = 'counter+arg='..(counter+arg)
        -- counter = counter + 1
        --> SCR_LOG Got do_math: do_math(0) 
        --> SCR_RET do_math 0 gave me counter+arg=100 

    elseif cmd == 'do_dbg' then
        dbg()
        ret = '!!!dbg()'
        --> SCR_RET do_dbg gave me !!!dbg() 
    elseif cmd == 'boom' then
        ret = boom()

    else
        ret = 'unknown cmd'
    end

    return ret
end
