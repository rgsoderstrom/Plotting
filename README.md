
Plotting Repository
===================

Wrappers around WPF and WPF3D plotting functions. These use my "Common" and "CommonMath" repos

Plot2D_Embedded        - 2D plot functions in a "bare" plot UserControl that must be enclosed within a Window
Plot2D_Embedded_Driver - unit test driver for Plot2D_Embedded. Also serves an example of how to use Plot2D_Embedded in a project

Plot3D_Embedded        - similar to Plot2D, except WPF3D plotting functions
Plot3D_Embedded_Driver - test driver and example for Plot3D

PlottingLib        - Puts a Window around Plot2D_Embedded or Plot3D_Embedded to make a standalone plot figure, like a Matlab figure
PlottingLib_Driver - test driver and examples of PlottingLib use
