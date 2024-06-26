FROM ubuntu:22.04

#install dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
git \
unzip \
wget \
sudo \
dotnet-sdk-6.0

#install godot engine
RUN wget https://github.com/godotengine/godot-builds/releases/download/4.2.2-stable/Godot_v4.2.2-stable_mono_linux_x86_64.zip \
--no-check-certificate
RUN unzip Godot_v4.2.2-stable_mono_linux_x86_64.zip
RUN mv Godot_v4.2.2-stable_mono_linux_x86_64 /usr/local/bin/godot \
&& rm -f Godot_v4.2.2-stable_mono_linux_x86_64.zip
RUN mv /usr/local/bin/godot/Godot_v4.2.2-stable_mono_linux.x86_64 \
/usr/local/bin/godot/godot-mono

#install godot export templates
RUN wget https://github.com/godotengine/godot-builds/releases/download/4.2.2-stable/Godot_v4.2.2-stable_mono_export_templates.tpz --no-check-certificate\
&& unzip Godot_v4.2.2-stable_mono_export_templates.tpz \
&& mv templates/* ~/.local/share/godot/export_templates/Godot_v4.2.2.stable_mono \
&& rm -f Godot_v4.2.2-stable_mono_export_templates.tpz

#download repo
RUN git clone https://github.com/hailey-pham/ConbiniGame.git
WORKDIR /ConbiniGame
RUN /usr/local/bin/godot/godot-mono --headless --export-release "Windows" /var/builds/project