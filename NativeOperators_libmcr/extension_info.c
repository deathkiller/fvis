#include "stdafx.h"
#include "common.h"

NATIVEOPERATORS_API extension_info get_extension_info() {
    extension_info info;
    info.flags = None;

    info.library_name = "Sun libmcr";
    info.version_major = 1;
    info.version_minor = 12;
    info.version_build = 0;

    return info;
}