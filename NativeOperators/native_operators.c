#include "stdafx.h"
#include "common.h"

#define _USE_MATH_DEFINES
#include <math.h>

NATIVEOPERATORS_API double constant_e() {
    return M_E;
}

NATIVEOPERATORS_API double constant_pi() {
    return M_PI;
}

NATIVEOPERATORS_API double operator_add(double x, double y) {
    return x + y;
}

NATIVEOPERATORS_API double operator_subtract(double x, double y) {
    return x - y;
}

NATIVEOPERATORS_API double operator_multiply(double x, double y) {
    return x * y;
}

NATIVEOPERATORS_API double operator_divide(double x, double y) {
    return x / y;
}

NATIVEOPERATORS_API double operator_pow(double x, double y) {
    return pow(x, y);
}

NATIVEOPERATORS_API double operator_remainder(double x, double y) {
    return fmod(x, y);
}

NATIVEOPERATORS_API double operator_abs(double x) {
    return fabs(x);
}

NATIVEOPERATORS_API double operator_sqrt(double x) {
    return sqrt(x);
}

NATIVEOPERATORS_API double operator_exp(double x) {
    return exp(x);
}

NATIVEOPERATORS_API double operator_ln(double x) {
    return log(x);
}

NATIVEOPERATORS_API double operator_log(double x) {
    return log10(x);
}

NATIVEOPERATORS_API double operator_sin(double x) {
    return sin(x);
}

NATIVEOPERATORS_API double operator_cos(double x) {
    return cos(x);
}

NATIVEOPERATORS_API double operator_tan(double x) {
    return tan(x);
}

NATIVEOPERATORS_API double operator_asin(double x) {
    return asin(x);
}

NATIVEOPERATORS_API double operator_acos(double x) {
    return acos(x);
}

NATIVEOPERATORS_API double operator_atan(double x) {
    return atan(x);
}

NATIVEOPERATORS_API double operator_sinh(double x) {
    return sinh(x);
}

NATIVEOPERATORS_API double operator_cosh(double x) {
    return cosh(x);
}

NATIVEOPERATORS_API double operator_tanh(double x) {
    return tanh(x);
}

NATIVEOPERATORS_API double operator_round(double x) {
#if defined(_MSC_VER)
    return (x < 0.0 ? ceil(x - 0.5) : floor(x + 0.5));
#else
    return round(x);
#endif
}

NATIVEOPERATORS_API double operator_floor(double x) {
    return floor(x);
}

NATIVEOPERATORS_API double operator_ceil(double x) {
    return ceil(x);
}