=== TODO
* move "trader" references to "merchant"
* add anti-aliasing
* debug why random seed doesn't work as expected
* make drawn lines centered on rotation point
* fix starting tail on lasers

=== ImageMagick reference
convert in.gif -layers Coalesce -resize 980x725 -layers Optimize out.gif
convert input.gif -coalesce -repage 0x0 -crop WxH+X+Y +repage output.gif
