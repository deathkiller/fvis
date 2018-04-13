#include "stdafx.h"
#include "common.h"

NATIVEOPERATORS_API double operator_sin(double x) {
    __asm {
        fld     x           // Push "x" onto the FPU register stack
        fsin                // Compute sin(x)
        fstp    x           // Copy the result from st(0) to "x" and pop register stack
    }
    return x;
}

NATIVEOPERATORS_API double operator_cos(double x) {
    __asm {
        fld     x           // Push "x" onto the FPU register stack
        fcos                // Compute cos(x)
        fstp    x           // Copy the result from st(0) to "x" and pop register stack
    }
    return x;
}

NATIVEOPERATORS_API double operator_tan(double x) {
    __asm {
        fld     x           // Push "x" onto the FPU register stack
        fptan               // Compute partial tan(x)
        fstp    st(0)       // fptan push "1" onto the FPU stack, so throw it away and pop register stack
        fstp    x           // Copy the result from st(0) to "x" and pop register stack
    }
    return x;
}