---@diagnostic disable: lowercase-global, unused-local, unused-function

-- Use the debugger. Note the project type must be `<OutputType>Exe</OutputType>` even if using WinForms.
local dbg = require("debugex")

-- Set any debugger config options here.
dbg.pretty_depth = 4
dbg.auto_where = 4
dbg.ansi_color = true
dbg.trace = false

-- Run debugger.
-- dbg.init(59120) -- socket
dbg.init() -- local cli

local li  = require("luainterop")

-- A var.
local counter = 100

-----------------------------------------------------------------------------
-- Log functions. These go straight through to the host. Magic numbers must match host code.
local function log_error(msg) li.log(4, msg) end
local function log_warn(msg)  li.log(3, msg) end
local function log_info(msg)  li.log(2, msg) end
local function log_debug(msg) li.log(1, msg) end
local function log_trace(msg) li.log(0, msg) end

log_info('Loading script_test.lua')

-- Test event.
li.notif(33, "Notification from script_test")

-- uncomment to cause .NET exception:
-- broken here


-----------------------------------------------------------------------------
function setup()
    log_info('setup() was called')
    return 3
end

-----------------------------------------------------------------------------
local function boom(tt)
    log_info('boom() was called - '..tt)
    local ret = 'boom'..nil
    return ret
end

-----------------------------------------------------------------------------
function do_command(cmd, arg)
    local ret = '???'

    log_info('Got do_command(): '..cmd..'('..arg..')')

    if cmd == 'do_math' then
        ret = '100 + arg='..(100 + arg)
    elseif cmd == 'do_dbg' then
        dbg()
        ret = 'dbg()!!!'
    elseif cmd == 'boom_exc' then
        -- Function that errors and throws .NET exceptions.
        ret = boom('shakalaka')
        log_info('boom_exc '..ret) -- never see this
    elseif cmd == 'boom_dbg' then
        -- Function that errors and runs debugger at error() site.
        res, msg = dbg.pcall(boom, 'lakashaka')
        log_info('boom_dbg '..ret) -- never see this
    else
        ret = 'unknown cmd'
    end

    return ret
end
