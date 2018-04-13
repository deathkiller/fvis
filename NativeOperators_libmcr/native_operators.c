#include "stdafx.h"
#include "common.h"

#define _USE_MATH_DEFINES
#include <math.h>
#include "libmcr/libmcr.h"

NATIVEOPERATORS_API double operator_pow(double x, double y) {
    return __libmcr_pow(x, y);
}

NATIVEOPERATORS_API double operator_exp(double x) {
    return __libmcr_exp(x);
}

NATIVEOPERATORS_API double operator_ln(double x) {
    return __libmcr_log(x);
}

NATIVEOPERATORS_API double operator_sin(double x) {
    return __libmcr_sin(x);
}

NATIVEOPERATORS_API double operator_cos(double x) {
    return __libmcr_cos(x);
}

NATIVEOPERATORS_API double operator_tan(double x) {
    return __libmcr_tan(x);
}

NATIVEOPERATORS_API double operator_atan(double x) {
    return __libmcr_atan(x);
}