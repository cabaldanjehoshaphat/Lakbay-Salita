"""
make_sfx.py - Sound effect sketches for Lakbay Salita (preview only, not used by the game yet).

Purpose: synthesizes the UI and gameplay sound effects (button clicks, panel open/close, sound on/off
toggles, keyboard/keypad taps, Wordle tile reveals, Crossword and Word Search feedback, win / time-up
jingles) with numpy, using the same Filipino-inspired timbres as the background music (bamboo
marimba, kulintang gong, bandurria pluck, bells) so music and effects sound like one game.

Usage:  python make_sfx.py      -> writes short mono .wav files into this folder
Each sound is one function in SOUNDS at the bottom; tweak pitch/decay there and run again.
"""
import os
import wave
import numpy as np

SR = 44100
OUT = os.path.dirname(os.path.abspath(__file__))
RNG = np.random.default_rng(11)


def t_(sec):
    return np.arange(int(sec * SR)) / SR


def hz(name):
    names = {"C": 0, "C#": 1, "D": 2, "D#": 3, "E": 4, "F": 5, "F#": 6, "G": 7, "G#": 8, "A": 9, "A#": 10, "B": 11}
    midi = 12 * (int(name[-1]) + 1) + names[name[:-1]]
    return 440.0 * 2 ** ((midi - 69) / 12)


def place(buf, sig, at):
    i = int(at * SR)
    end = min(len(buf), i + len(sig))
    buf[i:end] += sig[:end - i]
    return buf


def attack(sig, ms=2):
    n = max(1, int(ms / 1000 * SR))
    sig[:n] *= np.linspace(0, 1, n)
    return sig


# ------------------------------------------------------------------ timbres
def wood(f, dec=55, length=.12, bright=.5):  # bamboo tick / woodblock
    t = t_(length)
    s = np.sin(2 * np.pi * f * t) + bright * np.sin(2 * np.pi * 1.58 * f * t) * np.exp(-t * 30)
    s *= np.exp(-t * dec)
    s += RNG.standard_normal(len(t)) * .25 * np.exp(-t * 400)  # tiny contact click
    return attack(s, 1)


def marimba(f, length=.6, dec=7):
    t = t_(length)
    s = np.sin(2 * np.pi * f * t) * np.exp(-t * dec) + .3 * np.sin(2 * np.pi * 4 * f * t) * np.exp(-t * dec * 3)
    return attack(s, 1.5)


def bell(f, length=1.0, dec=4):
    t = t_(length)
    s = (np.sin(2 * np.pi * f * t) * np.exp(-t * dec) + .4 * np.sin(2 * np.pi * 2.76 * f * t) * np.exp(-t * dec * 1.8)
         + .15 * np.sin(2 * np.pi * 5.4 * f * t) * np.exp(-t * dec * 3))
    return attack(s, 1)


def gong(f, length=1.6, dec=2.6):
    t = t_(length)
    bend = 1 + .008 * np.exp(-t * 25)
    s = 0
    for r, a, d in [(1, 1, 1), (2.0, .3, 1.6), (2.76, .2, 2.3), (4.07, .1, 3.5)]:
        s = s + a * np.sin(2 * np.pi * f * r * t * bend) * np.exp(-t * dec * d)
    return attack(s, 2)


def pluck(f, length=.5, dec=6):
    t = t_(length)
    s = sum((.75 ** (k - 1)) / k * np.sin(2 * np.pi * f * k * t) * np.exp(-t * (dec + 2.5 * k)) for k in range(1, 8))
    return attack(s, 1)


def swish(length=.13, f0=1800, f1=5200):  # soft paper/tile flip
    n = int(length * SR)
    noise = RNG.standard_normal(n)
    t = np.arange(n) / SR
    # band-limited noise: moving average sized from a sweeping cutoff
    out = np.zeros(n)
    for i, w in enumerate(np.linspace(SR / f0, SR / f1, 8).astype(int)):
        seg = slice(i * n // 8, (i + 1) * n // 8)
        out[seg] = np.convolve(noise, np.ones(max(w, 1)) / max(w, 1), "same")[seg]
    out -= np.convolve(out, np.ones(60) / 60, "same")  # remove rumble
    return out * np.sin(np.pi * t / length) ** 2


def room(sig, wet=.12, rt=.35):
    n = int(rt * SR)
    t = np.arange(n) / SR
    ir = RNG.standard_normal(n) * np.exp(-6.9 * t / rt)
    ir /= np.sqrt(np.sum(ir ** 2))
    y = np.convolve(sig, ir)
    out = np.zeros(len(y))
    out[:len(sig)] += sig
    return out + wet * y


def buf(sec):
    return np.zeros(int(sec * SR))


# ------------------------------------------------------------------ sounds
# (file name, peak level 0..1, function) — quieter peaks for sounds that repeat a lot (keys, ticks)
def ui_click():
    return room(wood(1150, dec=60, length=.09, bright=.35), .08)

def ui_back():
    b = buf(.3); place(b, marimba(hz("G5"), .25, 16), 0); place(b, marimba(hz("D5"), .25, 14), .07); return room(b, .1)

def ui_open():
    b = buf(.5); place(b, bell(hz("C5"), .45, 7), 0); place(b, bell(hz("G5"), .45, 6), .07); return room(b, .15)

def ui_close():
    b = buf(.5); place(b, bell(hz("G5"), .45, 7), 0); place(b, bell(hz("C5"), .45, 6), .07); return room(b, .15)

def toggle_on():
    b = buf(.45); place(b, pluck(hz("E5"), .35, 9), 0); place(b, pluck(hz("A5"), .4, 7), .06); return room(b, .1)

def toggle_off():
    b = buf(.45); place(b, pluck(hz("A4"), .35, 10), 0); place(b, pluck(hz("E4"), .4, 9), .06); return room(b, .08)

def key_tap(f):
    return lambda: room(wood(f, dec=75, length=.07, bright=.3), .05)

def key_delete():
    t = t_(.1)
    ph = 2 * np.pi * np.cumsum(560 - 1800 * t) / SR
    s = (np.sin(ph) + .3 * np.sin(1.58 * ph)) * np.exp(-t * 45)
    return room(attack(s), .05)

def key_enter():
    b = buf(.45); place(b, marimba(hz("C5"), .4, 9), 0); place(b, marimba(hz("G5"), .4, 8), .045); return room(b, .1)

def word_invalid():
    b = buf(.5)
    for i, at in enumerate((0, .1)):
        t = t_(.18)
        f = 210 - 25 * i
        s = (np.sin(2 * np.pi * f * t) + .35 * np.sin(2 * np.pi * 2 * f * t)) * np.exp(-t * 16) * (1 + .35 * np.sin(2 * np.pi * 28 * t))
        place(b, attack(s, 3), at)
    return room(b, .06)

def tile_flip():
    return swish(.12)

def tile_correct():
    b = buf(.6); place(b, swish(.09) * .5, 0); place(b, bell(hz("A5"), .5, 8), .03); return room(b, .1)

def tile_present():
    b = buf(.5); place(b, swish(.09) * .5, 0); place(b, marimba(hz("E5"), .45, 9), .03); return room(b, .08)

def tile_absent():
    b = buf(.3); place(b, swish(.09) * .5, 0); place(b, wood(330, dec=40, length=.2, bright=.2) * .8, .03); return room(b, .05)

def win_jingle():
    b = buf(2.6)
    for i, n in enumerate(["D5", "E5", "F#5", "A5", "B5", "D6"]):
        place(b, gong(hz(n), 1.2, 3.2) * .8, i * .09)
    for n in ["D5", "F#5", "A5", "D6"]:
        place(b, bell(hz(n), 1.9, 1.6) * .5, .6)
    place(b, pluck(hz("D4"), 1.2, 3) * .6, .6)
    return room(b, .2, .9)

def time_up():
    b = buf(2.0)
    for i, n in enumerate(["A4", "F4", "D4"]):
        place(b, gong(hz(n), 1.4, 2.6) * .8, i * .22)
    return room(b, .18, .8)

def timer_tick():
    return room(wood(1700, dec=90, length=.05, bright=.2), .03)

def cw_cell_select():
    return room(pluck(hz("C6"), .18, 22), .05)

def cw_word_complete():
    b = buf(.9)
    for i, n in enumerate(["G5", "B5", "D6"]):
        place(b, bell(hz(n), .7, 5), i * .07)
    return room(b, .15, .6)

def ws_select():
    return room(marimba(hz("E5"), .22, 18), .05)

def ws_found():
    b = buf(1.1)
    for i, n in enumerate(["E6", "B5", "G5", "E5"][::-1] + ["B5", "E6"]):
        place(b, bell(hz(n), .6, 6) * (.6 if i < 4 else .8), i * .055)
    return room(b, .18, .7)

def ws_wrong():
    b = buf(.3); place(b, wood(260, dec=30, length=.25, bright=.15), 0); return room(b, .05)

def hint():
    b = buf(1.2)
    for i, n in enumerate(["C6", "E6", "G6", "C7", "E7"]):
        place(b, bell(hz(n), .7, 7) * (1 - i * .12), i * .045)
    return room(b, .3, .9)


SOUNDS = [
    ("ui_click", .5, ui_click), ("ui_back", .5, ui_back), ("ui_open", .55, ui_open), ("ui_close", .55, ui_close),
    ("toggle_on", .55, toggle_on), ("toggle_off", .5, toggle_off),
    ("key_tap_1", .38, key_tap(900)), ("key_tap_2", .38, key_tap(985)), ("key_tap_3", .38, key_tap(1070)),
    ("key_delete", .4, key_delete), ("key_enter", .55, key_enter), ("word_invalid", .55, word_invalid),
    ("tile_flip", .35, tile_flip), ("tile_correct", .55, tile_correct), ("tile_present", .5, tile_present), ("tile_absent", .45, tile_absent),
    ("win_jingle", .8, win_jingle), ("time_up", .7, time_up), ("timer_tick", .35, timer_tick),
    ("cw_cell_select", .4, cw_cell_select), ("cw_word_complete", .65, cw_word_complete),
    ("ws_select", .4, ws_select), ("ws_found", .65, ws_found), ("ws_wrong", .45, ws_wrong),
    ("hint", .6, hint),
]


def write(name, peak, sig):
    sig = sig / (np.max(np.abs(sig)) + 1e-9) * peak
    fade = min(len(sig), int(.01 * SR))
    sig[-fade:] *= np.linspace(1, 0, fade)
    path = os.path.join(OUT, name + ".wav")
    with wave.open(path, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes((sig * 32767).astype("<i2").tobytes())
    return len(sig) / SR


if __name__ == "__main__":
    for name, peak, fn in SOUNDS:
        print(f"{name:18s} {write(name, peak, fn()):.2f}s")
