#include "stdafx.h"
#include "common.h"

NATIVEOPERATORS_API extension_info get_extension_info() {
    extension_info info;
    info.flags = None;

    info.library_name = "x87 FPU";

    info.version_major = info.version_minor = info.version_build = 0;

    return info;
}