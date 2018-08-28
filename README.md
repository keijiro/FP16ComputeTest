FP16ComputeTest
---------------

This repository contains a simple program that tests performance of
half-precision floating point operations on DirectX 11/12 with using the
`min16float` type.

The following screenshot shows the result on RADEON RX 460 (click to enlarge).
The first two highlighted lines show the duration spent with large matrix
multiplication with the `float` type. The next two lines show the duration with
the same operation but with the `min16float` type.

<a href="https://i.imgur.com/HjnLkiz.png"><img src="https://i.imgur.com/HjnLkizl.jpg" /></a>

The next screenshot shows the result with the transposed matrix that improves
the performance thanks to memory locality.

<a href="https://i.imgur.com/uFB0b8o.png"><img src="https://i.imgur.com/uFB0b8ol.jpg" /></a>

It seems that using `min16float` can improve the performance despite the fact
that RX 460 doesn't have a FP16 pipeline.

The following screenshots show the results of the same program on GeForce GTX
1050 Ti. In these cases `min16float` gave negative effects. It seems that the
FP16 pipelines implemented in Pascal are not utilized on DirectX.

<a href="https://i.imgur.com/oUxskUA.png"><img src="https://i.imgur.com/oUxskUAl.jpg" /></a>

<a href="https://i.imgur.com/4D0pFtb.png"><img src="https://i.imgur.com/4D0pFtbl.jpg" /></a>

Please note that I'm not trying to provide accurate information. There are some
doubtful points in the results above -- why 1050 Ti can run x10 faster than RX
460? The only meaningful thing in these results is that you can't get any
performance boost by simply using `min16float`.
