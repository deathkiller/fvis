#pragma once

#if defined(_WIN32)
#   include "targetver.h"
#
#   define WIN32_LEAN_AND_MEAN      // Exclude rarely-used stuff from Windows headers
#   include <windows.h>
#endif