#!/bin/bash
for f in /mnt/c/Users/Colinou/tb/*;
do
	echo "Processing $f file..."
	filename=$(basename -- "$f")
	extension="${filename##*.}"
	filename="${filename%.*}"
	echo "$filename"
	ffmpeg -i "$f" -preset slow -n -filter:v scale=-2:480 -pix_fmt yuvj420p -crf 30  "/mnt/c/Users/Colinou/tb/md/""$filename""_md.mp4"
done
