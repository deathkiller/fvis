#pragma once

#ifndef NATIVEOPERATORS_CALLCONV
#   if defined(__GNUC__) && defined(__i386__) && !defined(__x86_64__)
#       define NATIVEOPERATORS_CALLCONV __attribute__((cdecl))
#   elif defined(_MSC_VER) && defined(_M_X86) && !defined(_M_X64)
#       define NATIVEOPERATORS_CALLCONV __cdecl
#   else
#       define NATIVEOPERATORS_CALLCONV
#   endif
#endif

#ifndef NATIVEOPERATORS_API
#   if defined(_WIN32) || defined(__CYGWIN__) || defined(__MINGW32__)
#       ifdef NATIVEOPERATORS_EXPORT_SYMBOLS
#           ifdef __GNUC__
#               define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV __attribute__((__dllexport__))
#           else
#               define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV __declspec(dllexport)
#           endif
#       else
#           ifdef __GNUC__
#               define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV __attribute__((__dllimport__))
#           else
#               define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV __declspec(dllimport)
#           endif
#       endif
#   else
#       if defined(__GNUC__) && __GNUC__ >= 4 && !defined(__CELLOS_LV2__)
#           define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV __attribute__((__visibility__("default")))
#       else
#           define NATIVEOPERATORS_API NATIVEOPERATORS_CALLCONV
#       endif
#    endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

typedef enum {
    None = 0,
    SSE = 1 << 0,
    SSE2 = 1 << 1,
    AVX = 1 << 2,
    AVX2 = 1 << 3,
} compiler_flags;

typedef struct {
    char* library_name;

    unsigned short version_major;
    unsigned short version_minor;
    unsigned short version_build;

    compiler_flags flags;
} extension_info;

NATIVEOPERATORS_API extension_info get_extension_info();

NATIVEOPERATORS_API double constant_e();
NATIVEOPERATORS_API double constant_pi();

NATIVEOPERATORS_API double operator_add(double x, double y);
NATIVEOPERATORS_API double operator_subtract(double x, double y);
NATIVEOPERATORS_API double operator_multiply(double x, double y);
NATIVEOPERATORS_API double operator_divide(double x, double y);
NATIVEOPERATORS_API double operator_pow(double x, double y);
NATIVEOPERATORS_API double operator_remainder(double x, double y);

NATIVEOPERATORS_API double operator_abs(double x);
NATIVEOPERATORS_API double operator_sqrt(double x);
NATIVEOPERATORS_API double operator_exp(double x);
NATIVEOPERATORS_API double operator_ln(double x);
NATIVEOPERATORS_API double operator_log(double x);
NATIVEOPERATORS_API double operator_sin(double x);
NATIVEOPERATORS_API double operator_cos(double x);
NATIVEOPERATORS_API double operator_tan(double x);
NATIVEOPERATORS_API double operator_asin(double x);
NATIVEOPERATORS_API double operator_acos(double x);
NATIVEOPERATORS_API double operator_atan(double x);
NATIVEOPERATORS_API double operator_sinh(double x);
NATIVEOPERATORS_API double operator_cosh(double x);
NATIVEOPERATORS_API double operator_tanh(double x);

NATIVEOPERATORS_API double operator_round(double x);
NATIVEOPERATORS_API double operator_floor(double x);
NATIVEOPERATORS_API double operator_ceil(double x);

#ifdef __cplusplus
}
#endif