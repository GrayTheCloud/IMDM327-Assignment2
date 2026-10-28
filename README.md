# IMDM 327 — Computational Virtual Reality

Derived from Unity course materials for **IMDM 327, Fall 2026**, taught by **Dr. Myungin Lee** at the University of Maryland. These examples are for classroom instruction, experimentation, and student projects.

[Course website](https://sites.google.com/umd.edu/imdm-327-lee-2026f/home)

## Introduction
My idea was to have the planets from the solar system simulation act as notes to create generative music derived from real-world data. This idea was inspired by NASA data sonification pieces like [5000 Exoplanets](https://www.youtube.com/watch?v=OOwI3nTAHIc). I was particularly interested in the way that the piece slowly built up over time and while it at times had a lot going on, the notes never clashed. I accomplished this in my piece by using a lydian major scale, which is often used in jazz improvisation as it has no “avoid note”. Each planet is assigned to a note on the scale and when its orbit is in a specific range, it “sings” its note, adding to the melody of the solar system. The differing orbits of the planets help create musicality, with closer planets with faster years acting as a steady beat while further planets play drawn out notes, chords form almost randomly as the different orbital speeds cause planets to synchronize and drift apart, making each simulation feel unique. 

##Methods
To make this piece, I combined the solar system simulation code that we created in class with the sound synthesizer example to make the planets themselves into the instrument. For modes of interaction, I was inspired by music boxes (which cyclically repeat their melodies on circular drums) and music creation software (where you individually place notes and adjust sound properties using sliders). I want the user to be able to set up their system and then see how it changes over time based on the different orbits of the planets, similar to making a song in a midi software then playing it back. 


##Demo

https://github.com/user-attachments/assets/dfaad820-d6ac-40d6-a55f-d316824eb941



## License

Course material code is released under the [MIT License](LICENSE). Third-party packages and assets retain their own licenses.
