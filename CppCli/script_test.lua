
local li  = require("luainterop")

-----------------------------------------------------------------------------
--- Log functions. This goes straight through to the host.
-- Magic numbers must match host code.
-- @param msg what to log
local function log_error(msg) li.log(4, msg) end
local function log_warn(msg)  li.log(3, msg) end
local function log_info(msg)  li.log(2, msg) end
local function log_debug(msg) li.log(1, msg) end
local function log_trace(msg) li.log(0, msg) end

log_info('Loading test.lua')

-- Test event.
li.notif(33, "Notification XYZ")


-----------------------------------------------------------------------------
function setup()
    log_info('setup() was called')
    -- log_info'aaa'
    return 3
end


-----------------------------------------------------------------------------
--- Global function for App interaction with script internals.
-- @param cmd specific command string
-- @param arg optional argument string
-- @return result string (table would be nice later)
function do_command(cmd, arg)
    local ret = 'OK'
    log_info('Got do_command: '..cmd..' '..arg)
    if cmd == 'unload_all' then  -- TODOF Unload everything so that the script can be reloaded.
        package.loaded.lbot_utils = nil
        package.loaded.stringex = nil
    elseif cmd == 'other_cmd' then -- As needed
        ret = "Later dude"
    else
        ret = 'Unknown cmd '..cmd..' '..arg
    end

    return ret
end
