goto skip_ftp
BIN
GET /pub/videolan/vlc/0.8.6/win32/vlc-0.8.6-win32.7z vlc.7z
QUIT

:skip_ftp
ftp -s:%0 -A downloads.videolan.org
copy /b 7z.sfx+vlc.7z vlc.exe
del vlc.7z