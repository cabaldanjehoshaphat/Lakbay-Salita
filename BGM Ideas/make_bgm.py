"""
make_bgm.py - Background music sketches for Lakbay Salita (preview only, not used by the game).

Purpose: composes one looping BGM track per screen (Main Menu, Profile, Language Selection, Library,
Wordle, Crossword, Word Search) with a small additive synth built on numpy. The instruments are
Filipino-inspired stand-ins: kulintang gongs, bandurria-style plucks, bamboo marimba (gabbang),
bamboo flute, plus soft pads, bass and light percussion. Every track loops seamlessly (the reverb
and note tails are wrapped back onto the start).

Usage:  python make_bgm.py            -> writes the .wav files into this folder
Edit the song functions at the bottom (tempo, chords, melodies) and run it again to regenerate.
"""
import os
import wave
import numpy as np

SR = 44100
RNG = np.random.default_rng(7)
OUT = os.path.dirname(os.path.abspath(__file__))

NOTE = {"C": 0, "C#": 1, "Db": 1, "D": 2, "D#": 3, "Eb": 3, "E": 4, "F": 5, "F#": 6, "Gb": 6,
        "G": 7, "G#": 8, "Ab": 8, "A": 9, "A#": 10, "Bb": 10, "B": 11}


def m(name):
    """Note name like 'F#4' -> MIDI number."""
    return 12 * (int(name[-1]) + 1) + NOTE[name[:-1]]


def hz(midi):
    return 440.0 * 2 ** ((midi - 69) / 12)


def tline(sec):
    return np.arange(int(sec * SR)) / SR


def fade(sig, attack, release, hold):
    """Linear attack, hold until `hold` seconds, then a release fade."""
    t = np.arange(len(sig)) / SR
    env = np.clip(t / max(attack, 1e-4), 0, 1)
    env *= np.clip(1 - (t - hold) / max(release, 1e-4), 0, 1)
    return sig * env


# ---------------------------------------------------------------- instruments (mono)
def gong(f, dur, vel):  # kulintang: bossed gong, inharmonic partials, small pitch drop
    t = tline(dur + 2.4)
    bend = 1 + 0.006 * np.exp(-t * 25)
    s = 0
    for ratio, amp, dec in [(1, 1, 2.0), (2.0, .3, 3.2), (2.76, .22, 4.5), (4.07, .1, 7), (5.4, .05, 10)]:
        s = s + amp * np.sin(2 * np.pi * f * ratio * t * bend) * np.exp(-t * dec)
    s += RNG.standard_normal(len(t)) * 0.08 * np.exp(-t * 90)
    return fade(s, .003, .6, dur + 1.6) * vel


def pluck(f, dur, vel, bright=1.0):  # bandurria / guitar-like
    t = tline(dur + 1.0)
    s = 0
    for k in range(1, 9):
        s = s + (bright ** (k - 1)) / k ** 1.1 * np.sin(2 * np.pi * f * k * t * (1 + .0007 * k)) * np.exp(-t * (1.6 + .9 * k))
    return fade(s, .002, .25, dur + .05) * vel


def marimba(f, dur, vel):  # bamboo xylophone (gabbang)
    t = tline(1.4)
    s = np.sin(2 * np.pi * f * t) * np.exp(-t * 4.5) + .35 * np.sin(2 * np.pi * 4 * f * t) * np.exp(-t * 14)
    s += .12 * np.sin(2 * np.pi * 9.2 * f * t) * np.exp(-t * 40)
    return fade(s, .002, .1, 1.3) * vel


def bell(f, dur, vel):  # music box / celesta
    t = tline(2.2)
    s = np.sin(2 * np.pi * f * t) * np.exp(-t * 2.6) + .35 * np.sin(2 * np.pi * 3 * f * t) * np.exp(-t * 5)
    s += .15 * np.sin(2 * np.pi * 5.4 * f * t) * np.exp(-t * 9)
    return fade(s, .002, .3, 1.9) * vel


def flute(f, dur, vel):  # bamboo flute with delayed vibrato and breath
    t = tline(dur + .25)
    vib = 1 + .006 * np.sin(2 * np.pi * 5.2 * t) * np.clip(t / .5, 0, 1)
    ph = 2 * np.pi * np.cumsum(f * vib) / SR
    s = np.sin(ph) + .22 * np.sin(2 * ph) + .07 * np.sin(3 * ph)
    breath = np.convolve(RNG.standard_normal(len(t)), np.ones(12) / 12, "same") * .05
    return fade(s + breath, .07, .22, dur) * vel


def pad(f, dur, vel):  # warm chorus pad
    t = tline(dur + 1.2)
    s = 0
    for cents in (-7, 0, 6):
        ff = f * 2 ** (cents / 1200)
        s = s + np.sin(2 * np.pi * ff * t) + .25 * np.sin(4 * np.pi * ff * t) + .1 * np.sin(6 * np.pi * ff * t)
    return fade(s / 3, .7, 1.1, dur) * vel


def epiano(f, dur, vel):  # soft electric piano
    t = tline(dur + .8)
    s = (np.sin(2 * np.pi * f * t) + .2 * np.sin(4 * np.pi * f * t)) * np.exp(-t * 1.1)
    s += .25 * np.sin(2 * np.pi * 7 * f * t) * np.exp(-t * 18)
    s *= 1 + .12 * np.sin(2 * np.pi * 4.5 * t)
    return fade(s, .003, .35, dur + .1) * vel


def bass(f, dur, vel):
    t = tline(dur + .2)
    s = (np.sin(2 * np.pi * f * t) + .35 * np.sin(4 * np.pi * f * t) + .1 * np.sin(6 * np.pi * f * t)) * np.exp(-t * 1.8)
    return fade(s, .004, .12, dur) * vel


def shaker(f, dur, vel):
    t = tline(.09)
    n = np.diff(RNG.standard_normal(len(t) + 1))
    return n * np.exp(-t * 55) * .35 * vel


def kick(f, dur, vel):
    t = tline(.35)
    ph = 2 * np.pi * np.cumsum(48 + 80 * np.exp(-t * 28)) / SR
    return np.sin(ph) * np.exp(-t * 11) * vel


def block(f, dur, vel):  # woodblock / bamboo click
    t = tline(.15)
    return (np.sin(2 * np.pi * f * t) + .5 * np.sin(2 * np.pi * 1.58 * f * t)) * np.exp(-t * 45) * vel


INSTR = dict(gong=gong, pluck=pluck, marimba=marimba, bell=bell, flute=flute, pad=pad,
             epiano=epiano, bass=bass, shaker=shaker, kick=kick, block=block)
PAN = dict(gong=.25, pluck=-.3, marimba=.3, bell=.2, flute=-.15, pad=0, epiano=-.1, bass=0,
           shaker=.4, kick=0, block=-.35)
LEVEL = dict(gong=.5, pluck=.32, marimba=.42, bell=.33, flute=.36, pad=.16, epiano=.3, bass=.5,
             shaker=.13, kick=.45, block=.18)


# ---------------------------------------------------------------- song building
class Song:
    def __init__(self, name, bpm, bars, wet=.22, room=1.6):
        self.name, self.bpm, self.bars, self.wet, self.room = name, bpm, bars, wet, room
        self.notes = []  # (beat, dur_beats, midi or freq, instrument, velocity)

    def add(self, beat, dur, note, inst, vel=1.0):
        self.notes.append((beat, dur, note, inst, vel))

    def phrase(self, start_bar, inst, items, shift=0, vel=1.0):
        """items: list of (beat_in_phrase, dur, 'C5')."""
        for b, d, n in items:
            self.add(start_bar * 4 + b, d, m(n) + shift, inst, vel)

    def render(self, path):
        spb = 60 / self.bpm
        length = int(self.bars * 4 * spb * SR)
        tail = int(4 * SR)
        L = np.zeros(length + tail)
        R = np.zeros(length + tail)
        for beat, dur, note, inst, vel in self.notes:
            # pitched notes are MIDI numbers; the woodblock takes a raw frequency in Hz
            f = note if inst == "block" else (hz(note) if note is not None else 0)
            human = RNG.normal(0, .006) if inst not in ("pad",) else 0
            v = vel * LEVEL[inst] * (1 + RNG.normal(0, .06))
            sig = INSTR[inst](f, dur * spb, v)
            start = int(max(0, (beat * spb + human)) * SR)
            end = min(start + len(sig), len(L))
            p = PAN[inst]
            L[start:end] += sig[:end - start] * np.cos((p + 1) * np.pi / 4)
            R[start:end] += sig[:end - start] * np.sin((p + 1) * np.pi / 4)
        L, R = reverb(L, self.room, self.wet, 1), reverb(R, self.room, self.wet, 2)
        # wrap the tail onto the start so the loop is seamless
        for ch in (L, R):
            ch[:tail] += ch[length:length + tail]
        st = np.stack([L[:length], R[:length]], 1)
        st = st / (np.sqrt(np.mean(st ** 2)) + 1e-9) * .11
        st = np.tanh(st * 1.4) / 1.4
        st = st / max(1.0, np.max(np.abs(st)) / .95)
        with wave.open(path, "wb") as w:
            w.setnchannels(2)
            w.setsampwidth(2)
            w.setframerate(SR)
            w.writeframes((st * 32767).astype("<i2").tobytes())
        print("wrote", os.path.basename(path), round(length / SR, 1), "s")


def reverb(x, rt, wet, seed):
    rng = np.random.default_rng(seed)
    n = int(rt * SR)
    t = np.arange(n) / SR
    ir = rng.standard_normal(n) * np.exp(-6.9 * t / rt)
    ir = np.convolve(ir, np.ones(6) / 6, "same")  # darken
    ir[:int(.012 * SR)] = 0
    ir /= np.sqrt(np.sum(ir ** 2))
    size = 1 << int(np.ceil(np.log2(len(x) + n)))
    y = np.fft.irfft(np.fft.rfft(x, size) * np.fft.rfft(ir, size), size)[:len(x)]
    return x + wet * y


def chords(song, prog, inst, octave_notes, pattern, vel=1.0, bars=None):
    """prog: list of chord tone lists (MIDI) per bar; pattern: list of (beat, dur, tone_index)."""
    for bar in (bars if bars is not None else range(song.bars)):
        tones = prog[bar % len(prog)]
        for b, d, i in pattern:
            song.add(bar * 4 + b, d, tones[i % len(tones)] + 12 * (i // len(tones)), inst, vel)


def triad(root, kind="maj", base=4):
    r = m(root + str(base))
    iv = dict(maj=[0, 4, 7], min=[0, 3, 7], maj7=[0, 4, 7, 11], min7=[0, 3, 7, 10], dom7=[0, 4, 7, 10], six=[0, 4, 7, 9])[kind]
    return [r + i for i in iv]


# ---------------------------------------------------------------- the seven tracks
def main_menu():
    s = Song("01_main_menu_lakbay", 96, 16, wet=.25)
    prog = [triad("D"), triad("B", "min", 3), triad("G", base=3), triad("A", base=3)]
    chords(s, prog, "pluck", None, [(0, .5, 0), (.5, .5, 1), (1, .5, 2), (1.5, .5, 1), (2, .5, 3), (2.5, .5, 2), (3, .5, 1), (3.5, .5, 2)], .9)
    for bar in range(16):
        root = prog[bar % 4][0] - 12 - (12 if prog[bar % 4][0] > m("F4") else 0)
        s.add(bar * 4, 1.5, root, "bass")
        s.add(bar * 4 + 2, 1.5, root + 7, "bass", .8)
        if bar >= 4:
            for e in range(8):
                s.add(bar * 4 + e * .5, .1, None, "shaker", 1 if e % 2 else .6)
        if bar >= 8:
            s.add(bar * 4, .5, None, "kick")
            s.add(bar * 4 + 2.5, .5, None, "kick", .7)
    A = [(0, 1, "A4"), (1, .5, "B4"), (1.5, .5, "A4"), (2, 1, "F#4"), (3, 1, "E4"), (4, 1.5, "F#4"), (5.5, .5, "A4"), (6, 2, "B4"),
         (8, 1, "D5"), (9, .5, "B4"), (9.5, .5, "A4"), (10, 1, "F#4"), (11, 1, "A4"), (12, 3, "E4")]
    B = [(0, 1, "A4"), (1, .5, "B4"), (1.5, .5, "D5"), (2, 1, "E5"), (3, 1, "D5"), (4, 1.5, "B4"), (5.5, .5, "A4"), (6, 2, "B4"),
         (8, 1, "D5"), (9, .5, "B4"), (9.5, .5, "A4"), (10, 1, "F#4"), (11, 1, "E4"), (12, 3, "D4")]
    s.phrase(4, "gong", A)
    s.phrase(8, "gong", B)
    s.phrase(12, "flute", A, shift=12)
    s.phrase(12, "gong", [(0, 1, "D4"), (4, 1, "B3"), (8, 1, "G3"), (12, 1, "A3")], vel=.7)
    return s


def profile():
    s = Song("02_profile_ako_ni", 84, 16, wet=.3, room=2.0)
    prog = [triad("F"), triad("D", "min"), triad("A#", base=3), triad("C")]
    for bar in range(16):
        tones = prog[bar % 4]
        for i, n in enumerate(tones):
            s.add(bar * 4, 4, n - 12, "pad", .9)
        s.add(bar * 4, 2, tones[0] - 24, "bass", .7)
        s.add(bar * 4 + 2, 2, tones[0] - 17, "bass", .5)
        chords(s, prog, "pluck", None, [(1, .5, 1), (1.5, .5, 2), (3, .5, 1), (3.5, .5, 2)], .45, bars=[bar])
    M = [(0, 1, "C5"), (1, 1, "A4"), (2, 1, "F5"), (3, 1, "E5"), (4, 1.5, "D5"), (5.5, .5, "C5"), (6, 2, "A4"),
         (8, 1, "A#4"), (9, 1, "D5"), (10, 1, "F5"), (11, 1, "D5"), (12, 2, "E5"), (14, 2, "G5"),
         (16, 1, "A5"), (17, 1, "G5"), (18, 1, "F5"), (19, 1, "C5"), (20, 2, "D5"), (22, 1, "F5"), (23, 1, "A5"),
         (24, 1.5, "G5"), (25.5, .5, "F5"), (26, 1, "D5"), (27, 1, "E5"), (28, 4, "F5")]
    s.phrase(0, "bell", M)
    s.phrase(8, "bell", M, vel=.9)
    s.phrase(8, "flute", [(0, 4, "A4"), (4, 4, "F4"), (8, 4, "F4"), (12, 4, "G4"), (16, 4, "C5"), (20, 4, "A4"), (24, 4, "A#4"), (28, 4, "A4")], vel=.55)
    return s


def language_selection():
    s = Song("03_language_selection_tulay", 104, 16, wet=.24)
    prog = [triad("G"), triad("E", "min"), triad("C"), triad("D")]
    chords(s, prog, "marimba", None, [(0, .5, 0), (.5, .5, 2), (1, .5, 1), (1.5, .5, 2), (2, .5, 3), (2.5, .5, 2), (3, .5, 1), (3.5, .5, 2)], .8)
    for bar in range(16):
        root = prog[bar % 4][0] - 24
        s.add(bar * 4, 1, root, "bass")
        s.add(bar * 4 + 1.5, .5, root, "bass", .6)
        s.add(bar * 4 + 2.5, 1, root + 7, "bass", .7)
        if bar >= 4:
            s.add(bar * 4 + 1, .2, 950, "block", .7)
            s.add(bar * 4 + 3, .2, 950, "block", .7)
    call = [(0, 1, "D5"), (1, .5, "B4"), (1.5, .5, "D5"), (2, 2, "E5"), (4, 1, "G5"), (5, 1, "E5"), (6, 2, "D5"),
            (8, 1, "C5"), (9, .5, "E5"), (9.5, .5, "G5"), (10, 2, "E5"), (12, 1, "D5"), (13, 1, "B4"), (14, 2, "A4")]
    answer = list(call[:-1]) + [(14, 2, "G4")]
    # three voices for the three languages: flute, bandurria, kulintang
    s.phrase(4, "flute", call)
    s.phrase(8, "pluck", answer, vel=1.2)
    s.phrase(12, "gong", call[:8] + [(8, 1, "C5"), (9, 1, "B4"), (10, 2, "A4"), (12, 4, "G4")], shift=-12)
    return s


def library():
    s = Song("04_library_tahimik", 70, 12, wet=.38, room=2.6)
    prog = [triad("A", "min7", 3), triad("F", "maj7", 3), triad("C", "maj7", 3), triad("G", "six", 3)]
    for bar in range(12):
        for n in prog[bar % 4]:
            s.add(bar * 4, 4, n, "pad", .8)
        s.add(bar * 4, 4, prog[bar % 4][0] - 12, "bass", .45)
    notes = [(0, 2, "E5"), (2, 1, "C5"), (3, 1, "B4"), (5, 2, "A4"), (6.5, 1.5, "C5"),
             (8, 2, "G4"), (10, 1, "E5"), (11, 1, "D5"), (13, 3, "E5")]
    s.phrase(0, "epiano", notes, vel=.8)
    s.phrase(4, "epiano", notes[:5] + [(8, 2, "B4"), (10, 2, "D5"), (13, 3, "C5")], vel=.75)
    s.phrase(8, "bell", [(0, 1, "E6"), (3, 1, "C6"), (6, 1, "B5"), (9, 1, "G5"), (12, 1, "A5"), (14, 1, "E5")], vel=.45)
    return s


def wordle():
    s = Song("05_wordle_hula", 100, 16, wet=.18)
    prog = [triad("C"), triad("A", "min", 3), triad("F", base=3), triad("G", base=3)]
    chords(s, prog, "pluck", None, [(0, .2, 0), (.5, .2, 1), (1, .2, 2), (2, .2, 1), (2.5, .2, 2), (3, .2, 3)], .8)
    for bar in range(16):
        root = prog[bar % 4][0] - (24 if prog[bar % 4][0] > m("D4") else 12)
        s.add(bar * 4, .5, root, "bass")
        s.add(bar * 4 + 1.5, .5, root, "bass", .7)
        s.add(bar * 4 + 2, .5, root + 7, "bass", .8)
        for b in (.5, 1.5, 2.5, 3.5):  # the "thinking clock"
            s.add(bar * 4 + b, .2, 1250, "block", .55)
        if bar >= 4:
            s.add(bar * 4, .5, None, "kick", .6)
            s.add(bar * 4 + 2, .5, None, "kick", .45)
    Q = [(0, .5, "E5"), (.5, .5, "G5"), (1, 1, "A5"), (2, .5, "G5"), (2.5, .5, "E5"), (3, 1, "D5"),
         (4, .5, "C5"), (4.5, .5, "E5"), (5, .5, "G5"), (5.5, .5, "B5"), (6, 2, "A5"),  # rising "question"
         (8, .5, "A5"), (8.5, .5, "G5"), (9, 1, "F5"), (10, .5, "E5"), (10.5, .5, "D5"), (11, 1, "C5"),
         (12, .5, "D5"), (12.5, .5, "E5"), (13, .5, "G5"), (13.5, .5, "D5"), (14, 2, "C5")]  # "answer"
    s.phrase(4, "bell", Q)
    s.phrase(8, "marimba", Q)
    s.phrase(12, "bell", Q, vel=.8)
    s.phrase(12, "marimba", Q, shift=-12, vel=.7)
    return s


def crossword():
    s = Song("06_crossword_pahalang_pababa", 88, 16, wet=.26, room=1.9)
    prog = [triad("D#", "maj7", 3), triad("C", "min7", 3), triad("G#", "maj7", 3), triad("A#", "dom7", 3)]
    chords(s, prog, "epiano", None, [(0, 1.4, 0), (0, 1.4, 1), (0, 1.4, 2), (0, 1.4, 3), (1.5, 2, 1), (1.5, 2, 2), (1.5, 2, 3)], .5)
    walk = {0: [0, 4, 7, 9], 1: [0, 3, 5, 7], 2: [0, 4, 7, 6], 3: [0, 2, 4, 5]}
    for bar in range(16):
        root = prog[bar % 4][0] - 12
        for i, iv in enumerate(walk[bar % 4]):
            s.add(bar * 4 + i, .9, root + iv - (12 if root > m("G3") else 0), "bass", .85)
        if bar >= 4:
            for b in (0, 1, 1.66, 2, 3, 3.66):  # swung brushes
                s.add(bar * 4 + b, .1, None, "shaker", .9 if b in (1, 3) else .55)
    F = [(0, 1.5, "G5"), (1.5, .5, "F5"), (2, 1, "D#5"), (3, 1, "D5"), (4, 2, "C5"), (6, 1, "D#5"), (7, 1, "G5"),
         (8, 1.5, "C6"), (9.5, .5, "A#5"), (10, 1, "G#5"), (11, 1, "G5"), (12, 3, "F5")]
    s.phrase(8, "flute", F, vel=.9)
    s.phrase(12, "flute", F[:10] + [(11, 1, "F5"), (12, 3, "D#5")], vel=.9)
    return s


def word_search():
    s = Song("07_word_search_pangita", 112, 16, wet=.2)
    prog = [triad("E", "min", 3), triad("C", base=3), triad("G", base=3), triad("D", base=3)]
    chords(s, prog, "marimba", None, [(0, .5, 0), (.5, .5, 1), (1, .5, 2), (1.5, .5, 3), (2, .5, 2), (2.5, .5, 1), (3, .5, 2), (3.5, .5, 4)], .75)
    for bar in range(16):
        root = prog[bar % 4][0] - 12
        for b, d, v in [(0, .75, 1), (.75, .25, .6), (1.5, .5, .8), (2.5, .5, .9), (3.5, .5, .6)]:
            s.add(bar * 4 + b, d, root - (12 if root > m("G2") else 0), "bass", v)
        s.add(bar * 4, .5, None, "kick", .8)
        s.add(bar * 4 + 2, .5, None, "kick", .6)
        if bar >= 2:
            for e in range(16):
                s.add(bar * 4 + e * .25, .05, None, "shaker", .9 if e % 4 == 2 else .4)
    motif = [(0, .5, "E5"), (.5, .5, "G5"), (1, .5, "A5"), (1.5, .5, "B5"), (2, 1, "D6"), (3, .5, "B5"), (3.5, .5, "A5")]
    reply = [(0, .5, "G4"), (.5, .5, "A4"), (1, 1, "B4"), (2, .5, "D5"), (2.5, .5, "B4"), (3, 1, "E4")]
    for bar in range(4, 16, 2):
        s.phrase(bar, "gong", motif, shift=-12, vel=.85)
        s.phrase(bar + 1, "pluck", reply, vel=1.1)
    s.phrase(12, "flute", [(0, 2, "B5"), (2, 2, "A5"), (4, 2, "G5"), (6, 2, "A5"), (8, 2, "B5"), (10, 2, "D6"), (12, 4, "E6")], vel=.7)
    return s


if __name__ == "__main__":
    for make in (main_menu, profile, language_selection, library, wordle, crossword, word_search):
        song = make()
        song.render(os.path.join(OUT, song.name + ".wav"))
