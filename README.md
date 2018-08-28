FP16ComputeTest
---------------

This repository contains a simple program that tests the performance of
half-precision floating point operations on DirectX11/12 with the
`min16float` type specifier.

The following screenshot shows the result on RADEON RX 460 (click to enlarge).
The first two highlighted lines show the duration spent by large matrix
multiplications with the `float` type. The next two lines show the duration by
the same operation but with the `min16float` type.

<a href="https://i.imgur.com/HjnLkiz.png"><img src="https://i.imgur.com/HjnLkizl.jpg" /></a>

The next screenshot shows the result with transposed matrices that improve the
performance thanks to data locality.

<a href="https://i.imgur.com/uFB0b8o.png"><img src="https://i.imgur.com/uFB0b8ol.jpg" /></a>

It seems that `min16float` improved the performance despite the fact that RX
460 doesn't have a FP16 pipeline.

The following screenshots show the results of the same program on GeForce GTX
1050 Ti. In these cases `min16float` gave negative effects. It seems that
Pascal's FP16 pipelines are not utilized for some reason.

<a href="https://i.imgur.com/oUxskUA.png"><img src="https://i.imgur.com/oUxskUAl.jpg" /></a>

<a href="https://i.imgur.com/4D0pFtb.png"><img src="https://i.imgur.com/4D0pFtbl.jpg" /></a>

Please note that I'm not trying to provide an accurate conclusion from these
results. You may find some doubtful points in them -- why 1050 Ti can run x10
faster than RX 460? The only meaningful conclusion from them is that you can't
get a quick performance boost by simply using `min16float`.
