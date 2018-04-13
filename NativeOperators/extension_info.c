#include "stdafx.h"
#include "common.h"

NATIVEOPERATORS_API extension_info get_extension_info() {
    extension_info info;
    info.flags = None;

#if defined(__clang__) // Clang/LLVM
    info.library_name = "Clang/LLVM";
    info.version_major = __clang_major__;
    info.version_minor = __clang_minor__;
    info.version_build = __clang_patchlevel__;
#elif defined(__ICC) || defined(__INTEL_COMPILER) // Intel ICC/ICPC
    info.library_name = "Intel ICC/ICPC";
    info.version_major = (__INTEL_COMPILER / 100);
    info.version_minor = (__INTEL_COMPILER % 100);
    info.version_build = 0;
#elif defined(__HP_cc) || defined(__HP_aCC) // Hewlett-Packard C/aC++
    info.library_name = "Hewlett-Packard C/aC++";
    info.version_major = info.version_minor = info.version_build = 0;
#elif defined(__GNUG__) // GNU G++
    info.library_name = "GNU G++";
    info.version_major = __GNUG__;
    info.version_minor = __GNUC_MINOR__;
    info.version_build = __GNUC_PATCHLEVEL__;
#elif defined(__GNUC__) // GNU GCC
    info.library_name = "GNU GCC";
    info.version_major = __GNUC__;
    info.version_minor = __GNUC_MINOR__;
    info.version_build = __GNUC_PATCHLEVEL__;
#elif defined(__IBMC__) || defined(__IBMCPP__) // IBM XL C/C++
    info.library_name = "IBM XL C/C++";
    info.version_major = info.version_minor = info.version_build = 0;
#elif defined(_MSC_VER) // Microsoft Visual Studio
    info.library_name = "MSVC";
    info.version_major = (_MSC_VER / 100);
    info.version_minor = (_MSC_VER % 100);
    info.version_build = 0;

#   if defined(_M_IX86_FP)
#       if (_M_IX86_FP == 0)
            // None
#       elif (_M_IX86_FP == 1)
            // SSE
            info.flags |= SSE;
#       elif (_M_IX86_FP == 2)
#           if defined(__AVX2__)
                // AVX2
                info.flags |= AVX2;
#           elif defined(__AVX__)
                // AVX
                info.flags |= AVX;
#           else
                // SSE2
                info.flags |= SSE2;
#           endif
#       endif
#   endif

#elif defined(__PGI) // Portland Group PGCC/PGCPP
    info.library_name = "Portland Group PGCC/PGCPP";
    info.version_major = __PGIC__;
    info.version_minor = __PGIC_MINOR__;
    info.version_build = __PGIC_PATCHLEVEL__;
#elif defined(__SUNPRO_C) || defined(__SUNPRO_CC) // Oracle Solaris Studio
    info.library_name = "Oracle Solaris Studio";
    info.version_major = info.version_minor = info.version_build = 0;
#else
    info.library_name = "Unknown";
    info.version_major = info.version_minor = info.version_build = 0;
#endif
    return info;
}